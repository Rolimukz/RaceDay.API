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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/results/me (Participant)
        [HttpGet("me")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var results = await _context.Results
                .Where(r => r.UserID == userId)
                .Include(r => r.Event)
                .Include(r => r.Category)
                .Select(r => new
                {
                    r.ResultID,
                    r.FinishTime,
                    r.OverallPosition,
                    r.CategoryPosition,
                    r.Status,
                    Event = r.Event != null ? new
                    {
                        r.Event.EventID,
                        r.Event.EventName,
                        r.Event.EventDate,
                        r.Event.City,
                        r.Event.Province
                    } : null,
                    Category = r.Category != null ? new
                    {
                        r.Category.CategoryID,
                        r.Category.CategoryName,
                        r.Category.Distance
                    } : null
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/events/{eventId}/results (Public)
        [HttpGet("/api/events/{eventId}/results")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            var results = await _context.Results
                .Where(r => r.EventID == eventId)
                .Include(r => r.User)
                .Include(r => r.Category)
                .OrderBy(r => r.OverallPosition)
                .Select(r => new
                {
                    r.ResultID,
                    r.FinishTime,
                    r.OverallPosition,
                    r.CategoryPosition,
                    r.Status,
                    Participant = r.User != null ? new
                    {
                        r.User.UserID,
                        r.User.FirstName,
                        r.User.LastName
                    } : null,
                    Category = r.Category != null ? new
                    {
                        r.Category.CategoryID,
                        r.Category.CategoryName,
                        r.Category.Distance
                    } : null
                })
                .ToListAsync();

            return Ok(results);
        }

        // POST: api/enrolments/{enrolmentId}/results (Organiser owner only)
        [HttpPost("/api/enrolments/{enrolmentId}/results")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> CreateResult(int enrolmentId, [FromBody] CreateResultDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == enrolmentId);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found" });

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (enrolment.Event?.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            // Check if result already exists
            var existingResult = await _context.Results
                .FirstOrDefaultAsync(r => r.EnrolmentID == enrolmentId);

            if (existingResult != null)
                return Conflict(new { message = "Result already exists for this enrolment" });

            var result = new Result
            {
                EnrolmentID = enrolmentId,
                UserID = enrolment.UserID,
                EventID = enrolment.EventID,
                CategoryID = enrolment.CategoryID,
                FinishTime = request.FinishTime,
                OverallPosition = request.OverallPosition,
                CategoryPosition = request.CategoryPosition,
                Status = request.Status ?? "Completed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Results.Add(result);

            // Update enrolment status to Completed
            enrolment.Status = "Completed";
            enrolment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventResults), new { eventId = result.EventID }, new
            {
                result.ResultID,
                result.EnrolmentID,
                result.FinishTime,
                result.OverallPosition,
                result.CategoryPosition,
                result.Status,
                message = "Result created successfully"
            });
        }

        // PUT: api/results/{id} (Organiser owner only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpdateResult(int id, [FromBody] UpdateResultDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _context.Results
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
                return NotFound(new { message = "Result not found" });

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (result.Event?.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            if (request.FinishTime.HasValue)
                result.FinishTime = request.FinishTime.Value;
            if (request.OverallPosition.HasValue)
                result.OverallPosition = request.OverallPosition.Value;
            if (request.CategoryPosition.HasValue)
                result.CategoryPosition = request.CategoryPosition.Value;
            if (!string.IsNullOrEmpty(request.Status))
                result.Status = request.Status;

            result.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Result updated successfully" });
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