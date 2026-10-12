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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/categories/{id} (Public)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(new
            {
                category.CategoryID,
                category.CategoryName,
                category.Distance,
                category.StartTime,
                category.AgeGroup,
                category.EntryFee,
                category.MaxParticipants,
                Event = category.Event != null ? new
                {
                    category.Event.EventID,
                    category.Event.EventName,
                    category.Event.EventDate
                } : null
            });
        }

        // PUT: api/categories/{id} (Organiser owner only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return NotFound(new { message = "Category not found" });

            // Check if user is the organiser of the event or admin
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (category.Event?.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            if (!string.IsNullOrEmpty(request.CategoryName))
                category.CategoryName = request.CategoryName;
            if (request.Distance.HasValue)
                category.Distance = request.Distance.Value;
            if (request.StartTime.HasValue)
                category.StartTime = request.StartTime.Value;
            if (!string.IsNullOrEmpty(request.AgeGroup))
                category.AgeGroup = request.AgeGroup;
            if (request.EntryFee.HasValue)
                category.EntryFee = request.EntryFee.Value;
            if (request.MaxParticipants.HasValue)
                category.MaxParticipants = request.MaxParticipants.Value;

            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully" });
        }

        // DELETE: api/categories/{id} (Organiser owner only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return NotFound(new { message = "Category not found" });

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (category.Event?.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully" });
        }

        // GET: api/categories/{id}/results (Public)
        [HttpGet("{id}/results")]
        public async Task<IActionResult> GetCategoryResults(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            var results = await _context.Results
                .Where(r => r.CategoryID == id)
                .Include(r => r.User)
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
                    } : null
                })
                .ToListAsync();

            return Ok(results);
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