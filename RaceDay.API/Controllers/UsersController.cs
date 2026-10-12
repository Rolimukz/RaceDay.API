using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.UserID,
                user.Email,
                user.FirstName,
                user.LastName,
                user.DateOfBirth,
                user.Gender,
                user.PhoneNumber,
                user.ProfileImage,
                user.IsActive,
                user.CreatedAt,
                Role = user.Role?.RoleName
            });
        }

        // PUT: api/users/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if (!string.IsNullOrEmpty(request.FirstName))
                user.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                user.LastName = request.LastName;
            if (request.DateOfBirth.HasValue)
                user.DateOfBirth = request.DateOfBirth.Value;
            if (!string.IsNullOrEmpty(request.Gender))
                user.Gender = request.Gender;
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrEmpty(request.ProfileImage))
                user.ProfileImage = request.ProfileImage;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully" });
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.UserID,
                user.Email,
                user.FirstName,
                user.LastName,
                user.DateOfBirth,
                user.Gender,
                user.PhoneNumber,
                user.ProfileImage,
                user.IsActive,
                Role = user.Role?.RoleName
            });
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
