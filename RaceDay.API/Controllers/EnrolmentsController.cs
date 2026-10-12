using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/enrolments/me (Participant)
        [HttpGet("me")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var enrolments = await _context.Enrolments
                .Where(e => e.UserID == userId)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Include(e => e.Result)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.EnrolmentDate,
                    e.Status,
                    e.PaymentStatus,
                    e.BibNumber,
                    e.VolunteerRole,
                    e.MedicalNotes,
                    Event = e.Event != null ? new
                    {
                        e.Event.EventID,
                        e.Event.EventName,
                        e.Event.EventDate,
                        e.Event.City,
                        e.Event.Province
                    } : null,
                    Category = e.Category != null ? new
                    {
                        e.Category.CategoryID,
                        e.Category.CategoryName,
                        e.Category.Distance,
                        e.Category.StartTime
                    } : null,
                    Result = e.Result != null ? new
                    {
                        e.Result.FinishTime,
                        e.Result.OverallPosition,
                        e.Result.CategoryPosition,
                        e.Result.Status
                    } : null
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // POST: api/events/{eventId}/enrolments (Participant)
        [HttpPost("/api/events/{eventId}/enrolments")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> EnrolInEvent(int eventId, [FromBody] CreateEnrolmentDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Check if event exists
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            // Check if category exists and belongs to the event
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == request.CategoryID && c.EventID == eventId);

            if (category == null)
                return NotFound(new { message = "Category not found for this event" });

            // Check if already enrolled
            var existingEnrolment = await _context.Enrolments
                .FirstOrDefaultAsync(e => e.UserID == userId && e.EventID == eventId);

            if (existingEnrolment != null)
                return Conflict(new { message = "You are already enrolled in this event" });

            // Check if category is full
            var currentEnrolments = await _context.Enrolments
                .CountAsync(e => e.CategoryID == request.CategoryID);

            if (currentEnrolments >= category.MaxParticipants)
                return BadRequest(new { message = "This category is full" });

            // Generate Bib Number
            var bibNumber = $"BIB{eventId:D3}{userId:D3}";

            var enrolment = new Enrolment
            {
                UserID = userId.Value,
                EventID = eventId,
                CategoryID = request.CategoryID,
                EnrolmentDate = DateTime.UtcNow,
                Status = "Registered",
                PaymentStatus = "Pending",
                BibNumber = bibNumber,
                VolunteerRole = request.VolunteerRole,
                MedicalNotes = request.MedicalNotes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyEnrolments), new { id = enrolment.EnrolmentID }, new
            {
                enrolment.EnrolmentID,
                enrolment.EventID,
                enrolment.CategoryID,
                enrolment.Status,
                enrolment.PaymentStatus,
                enrolment.BibNumber,
                message = "Enrolled successfully"
            });
        }

        // PUT: api/enrolments/{id} (Participant)
        [HttpPut("{id}")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> UpdateEnrolment(int id, [FromBody] UpdateEnrolmentDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var enrolment = await _context.Enrolments.FindAsync(id);
            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found" });

            if (enrolment.UserID != userId)
                return Forbid();

            if (!string.IsNullOrEmpty(request.Status))
                enrolment.Status = request.Status;
            if (!string.IsNullOrEmpty(request.PaymentStatus))
                enrolment.PaymentStatus = request.PaymentStatus;
            if (!string.IsNullOrEmpty(request.VolunteerRole))
                enrolment.VolunteerRole = request.VolunteerRole;
            if (!string.IsNullOrEmpty(request.MedicalNotes))
                enrolment.MedicalNotes = request.MedicalNotes;

            enrolment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolment updated successfully" });
        }

        // DELETE: api/enrolments/{id} (Participant)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> WithdrawFromEvent(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var enrolment = await _context.Enrolments.FindAsync(id);
            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found" });

            if (enrolment.UserID != userId)
                return Forbid();

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Withdrawn from event successfully" });
        }

        // GET: api/events/{eventId}/enrolments (Organiser owner only)
        [HttpGet("/api/events/{eventId}/enrolments")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (eventItem.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            var enrolments = await _context.Enrolments
                .Where(e => e.EventID == eventId)
                .Include(e => e.User)
                .Include(e => e.Category)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.EnrolmentDate,
                    e.Status,
                    e.PaymentStatus,
                    e.BibNumber,
                    e.VolunteerRole,
                    e.MedicalNotes,
                    Participant = e.User != null ? new
                    {
                        e.User.UserID,
                        e.User.FirstName,
                        e.User.LastName,
                        e.User.Email,
                        e.User.PhoneNumber
                    } : null,
                    Category = e.Category != null ? new
                    {
                        e.Category.CategoryID,
                        e.Category.CategoryName,
                        e.Category.Distance
                    } : null
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            return int.Parse(userIdClaim);
        }
    }
}