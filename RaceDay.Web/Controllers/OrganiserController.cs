using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{
    public class OrganiserController : BaseController
    {
        public OrganiserController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        // GET: /Organiser/Index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Organiser")
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

        // GET: /Organiser/CreateEvent
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        // POST: /Organiser/CreateEvent
        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = GetAuthenticatedClient();

            // Use MultipartFormDataContent to send both text and files
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.EventName), "EventName");
            content.Add(new StringContent(model.Description ?? ""), "Description");
            content.Add(new StringContent(model.EventDate.ToString("o")), "EventDate");
            content.Add(new StringContent(model.Venue ?? ""), "Venue");
            content.Add(new StringContent(model.City), "City");
            content.Add(new StringContent(model.Province), "Province");
            content.Add(new StringContent(model.EntryFee.ToString()), "EntryFee");
            content.Add(new StringContent(model.MaxParticipants.ToString()), "MaxParticipants");
            content.Add(new StringContent(model.Status), "Status");

            // If an image was uploaded, add it to the request
            if (model.BannerImage != null && model.BannerImage.Length > 0)
            {
                var fileContent = new StreamContent(model.BannerImage.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.BannerImage.ContentType);
                content.Add(fileContent, "BannerImage", model.BannerImage.FileName);
            }

            var response = await client.PostAsync("events", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Failed to create event: {error}");
            return View(model);
        }
    }
}