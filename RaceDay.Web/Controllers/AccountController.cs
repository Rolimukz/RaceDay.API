using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseViewModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (authResponse != null)
                {
                    // Store user details in Session
                    HttpContext.Session.SetString("JwtToken", authResponse.Token);
                    HttpContext.Session.SetString("UserRole", authResponse.Role);
                    HttpContext.Session.SetString("UserName", $"{authResponse.FirstName} {authResponse.LastName}");
                    HttpContext.Session.SetInt32("UserID", authResponse.UserID);

                    // Redirect based on role
                    if (authResponse.Role == "Organiser") return RedirectToAction("Index", "Organiser");
                    if (authResponse.Role == "Participant") return RedirectToAction("Index", "Participant");
                }
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction(nameof(Login));
            }

            var errorString = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Registration failed. Email might already exist.");
            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}