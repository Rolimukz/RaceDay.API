using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace RaceDay.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IHttpClientFactory _httpClientFactory;

        public BaseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Helper method to get an HttpClient with the JWT token attached
        protected HttpClient GetAuthenticatedClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");
            var token = HttpContext.Session.GetString("JwtToken");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}