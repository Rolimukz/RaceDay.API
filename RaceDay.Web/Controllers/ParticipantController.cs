using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{
    public class ParticipantController : BaseController
    {
        public ParticipantController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        // GET: /Participant/Index (Browse Events)
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant")
                return RedirectToAction("Login", "Account");

            var client = GetAuthenticatedClient();
            var response = await client.GetAsync("events");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var events = JsonSerializer.Deserialize<List<EventViewModel>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(events);
            }

            return View(new List<EventViewModel>());
        }

        // GET: /Participant/MyEvents
        public async Task<IActionResult> MyEvents()
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant")
                return RedirectToAction("Login", "Account");

            var client = GetAuthenticatedClient();
            var response = await client.GetAsync("enrolments/me");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var enrolments = JsonSerializer.Deserialize<List<EnrolmentViewModel>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(enrolments);
            }

            return View(new List<EnrolmentViewModel>());
        }

        // GET: /Participant/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant")
                return RedirectToAction("Login", "Account");

            var client = GetAuthenticatedClient();
            var response = await client.GetAsync($"events/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                // Deserialize into a detailed view model that includes Categories
                var eventDetails = JsonSerializer.Deserialize<EventDetailsViewModel>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(eventDetails);
            }

            return NotFound();
        }

        // POST: /Participant/Enrol
        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant")
                return RedirectToAction("Login", "Account");

            var client = GetAuthenticatedClient();

            // Prepare request body
            var enrolRequest = new { CategoryID = categoryId, VolunteerRole = "", MedicalNotes = "" };
            var jsonContent = new StringContent(JsonSerializer.Serialize(enrolRequest), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"events/{eventId}/enrolments", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "You have successfully enrolled in the event!";
                return RedirectToAction(nameof(MyEvents));
            }

            var error = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Failed to enrol: {error}";
            return RedirectToAction(nameof(Details), new { id = eventId });
        }
    }
}