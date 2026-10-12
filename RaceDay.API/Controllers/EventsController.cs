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
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/events (Public)
        [HttpGet]
        public async Task<IActionResult> GetAllEvents([FromQuery] string? status, [FromQuery] string? city, [FromQuery] string? province)
        {
            var query = _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(e => e.Status == status);
            if (!string.IsNullOrEmpty(city))
                query = query.Where(e => e.City.Contains(city));
            if (!string.IsNullOrEmpty(province))
                query = query.Where(e => e.Province.Contains(province));

            var events = await query.Select(e => new
            {
                e.EventID,
                e.EventName,
                e.Description,
                e.EventDate,
                e.Venue,
                e.City,
                e.Province,
                e.EntryFee,
                e.MaxParticipants,
                e.Status,
                Organiser = e.Organiser != null ? $"{e.Organiser.FirstName} {e.Organiser.LastName}" : null,
                TotalCategories = e.Categories != null ? e.Categories.Count : 0
            }).ToListAsync();

            return Ok(events);
        }

        // GET: api/events/upcoming (Public)
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingEvents()
        {
            var events = await _context.Events
                .Where(e => e.EventDate >= DateTime.UtcNow && e.Status == "Open")
                .OrderBy(e => e.EventDate)
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.EventDate,
                    e.City,
                    e.Province,
                    e.EntryFee,
                    e.Status
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/past (Public)
        [HttpGet("past")]
        public async Task<IActionResult> GetPastEvents()
        {
            var events = await _context.Events
                .Where(e => e.EventDate < DateTime.UtcNow || e.Status == "Completed")
                .OrderByDescending(e => e.EventDate)
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.EventDate,
                    e.City,
                    e.Province,
                    e.Status
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/search (Public)
        [HttpGet("search")]
        public async Task<IActionResult> SearchEvents([FromQuery] string? q, [FromQuery] string? city, [FromQuery] string? province, [FromQuery] DateTime? date)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(q))
                query = query.Where(e => e.EventName.Contains(q) || e.Description!.Contains(q));
            if (!string.IsNullOrEmpty(city))
                query = query.Where(e => e.City.Contains(city));
            if (!string.IsNullOrEmpty(province))
                query = query.Where(e => e.Province.Contains(province));
            if (date.HasValue)
                query = query.Where(e => e.EventDate.Date == date.Value.Date);

            var events = await query.Select(e => new
            {
                e.EventID,
                e.EventName,
                e.EventDate,
                e.City,
                e.Province,
                e.EntryFee,
                e.Status
            }).ToListAsync();

            return Ok(events);
        }

        // GET: api/events/{id} (Public)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .Include(e => e.EventRoute)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            return Ok(new
            {
                eventItem.EventID,
                eventItem.EventName,
                eventItem.Description,
                eventItem.EventDate,
                eventItem.Venue,
                eventItem.City,
                eventItem.Province,
                eventItem.EntryFee,
                eventItem.MaxParticipants,
                eventItem.Status,
                Organiser = eventItem.Organiser != null ? new
                {
                    eventItem.Organiser.UserID,
                    eventItem.Organiser.FirstName,
                    eventItem.Organiser.LastName,
                    eventItem.Organiser.Email
                } : null,
                Categories = eventItem.Categories?.Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.Distance,
                    c.StartTime,
                    c.AgeGroup,
                    c.EntryFee,
                    c.MaxParticipants
                }),
                Route = eventItem.EventRoute != null ? new
                {
                    eventItem.EventRoute.RouteID,
                    eventItem.EventRoute.RouteName,
                    eventItem.EventRoute.StartPoint,
                    eventItem.EventRoute.EndPoint,
                    eventItem.EventRoute.Distance,
                    eventItem.EventRoute.ElevationGain,
                    eventItem.EventRoute.RouteMapURL,
                    eventItem.EventRoute.GPXFileURL
                } : null
            });
        }

        // POST: api/events (Organiser only)
        [HttpPost]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var eventItem = new Event
            {
                OrganiserID = userId.Value,
                EventName = request.EventName,
                Description = request.Description,
                EventDate = request.EventDate,
                Venue = request.Venue,
                City = request.City,
                Province = request.Province,
                EntryFee = request.EntryFee,
                MaxParticipants = request.MaxParticipants,
                Status = request.Status ?? "Draft",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventById), new { id = eventItem.EventID }, new
            {
                eventItem.EventID,
                eventItem.EventName,
                eventItem.EventDate,
                eventItem.City,
                eventItem.Province,
                eventItem.Status,
                message = "Event created successfully"
            });
        }

        // PUT: api/events/{id} (Organiser owner only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            // Check if user is the organiser or admin
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (eventItem.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            if (!string.IsNullOrEmpty(request.EventName))
                eventItem.EventName = request.EventName;
            if (request.Description != null)
                eventItem.Description = request.Description;
            if (request.EventDate.HasValue)
                eventItem.EventDate = request.EventDate.Value;
            if (!string.IsNullOrEmpty(request.Venue))
                eventItem.Venue = request.Venue;
            if (!string.IsNullOrEmpty(request.City))
                eventItem.City = request.City;
            if (!string.IsNullOrEmpty(request.Province))
                eventItem.Province = request.Province;
            if (request.EntryFee.HasValue)
                eventItem.EntryFee = request.EntryFee.Value;
            if (request.MaxParticipants.HasValue)
                eventItem.MaxParticipants = request.MaxParticipants.Value;
            if (!string.IsNullOrEmpty(request.Status))
                eventItem.Status = request.Status;

            eventItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully" });
        }

        // DELETE: api/events/{id} (Organiser owner only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (eventItem.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event deleted successfully" });
        }

        // GET: api/events/{id}/categories (Public)
        [HttpGet("{id}/categories")]
        public async Task<IActionResult> GetEventCategories(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            var categories = await _context.Categories
                .Where(c => c.EventID == id)
                .Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.Distance,
                    c.StartTime,
                    c.AgeGroup,
                    c.EntryFee,
                    c.MaxParticipants
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/events/{id}/categories (Organiser owner only)
        [HttpPost("{id}/categories")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> AddCategory(int id, [FromBody] CreateCategoryDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (eventItem.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            var category = new Category
            {
                EventID = id,
                CategoryName = request.CategoryName,
                Distance = request.Distance,
                StartTime = request.StartTime,
                AgeGroup = request.AgeGroup,
                EntryFee = request.EntryFee,
                MaxParticipants = request.MaxParticipants,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventCategories), new { id = category.CategoryID }, new
            {
                category.CategoryID,
                category.CategoryName,
                category.Distance,
                message = "Category added successfully"
            });
        }

        // GET: api/events/{id}/route (Public)
        [HttpGet("{id}/route")]
        public async Task<IActionResult> GetEventRoute(int id)
        {
            var eventRoute = await _context.EventRoutes
                .FirstOrDefaultAsync(er => er.EventID == id);

            if (eventRoute == null)
                return NotFound(new { message = "Route not found for this event" });

            return Ok(new
            {
                eventRoute.RouteID,
                eventRoute.RouteName,
                eventRoute.StartPoint,
                eventRoute.EndPoint,
                eventRoute.Distance,
                eventRoute.ElevationGain,
                eventRoute.RouteMapURL,
                eventRoute.GPXFileURL
            });
        }

        // PUT: api/events/{id}/route (Organiser owner only)
        [HttpPut("{id}/route")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateRouteDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (eventItem.OrganiserID != userId && userRole != "Admin")
                return Forbid();

            var eventRoute = await _context.EventRoutes
                .FirstOrDefaultAsync(er => er.EventID == id);

            if (eventRoute == null)
            {
                eventRoute = new EventRoute
                {
                    EventID = id,
                    RouteName = request.RouteName,
                    StartPoint = request.StartPoint,
                    EndPoint = request.EndPoint,
                    Distance = request.Distance,
                    ElevationGain = request.ElevationGain,
                    RouteMapURL = request.RouteMapURL,
                    GPXFileURL = request.GPXFileURL,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.EventRoutes.Add(eventRoute);
            }
            else
            {
                if (!string.IsNullOrEmpty(request.RouteName))
                    eventRoute.RouteName = request.RouteName;
                if (!string.IsNullOrEmpty(request.StartPoint))
                    eventRoute.StartPoint = request.StartPoint;
                if (!string.IsNullOrEmpty(request.EndPoint))
                    eventRoute.EndPoint = request.EndPoint;
                if (request.Distance > 0)
                    eventRoute.Distance = request.Distance;
                if (request.ElevationGain.HasValue)
                    eventRoute.ElevationGain = request.ElevationGain;
                if (!string.IsNullOrEmpty(request.RouteMapURL))
                    eventRoute.RouteMapURL = request.RouteMapURL;
                if (!string.IsNullOrEmpty(request.GPXFileURL))
                    eventRoute.GPXFileURL = request.GPXFileURL;

                eventRoute.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Route updated successfully" });
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