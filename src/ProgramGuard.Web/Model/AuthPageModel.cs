using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ProgramGuard.Web.Model
{
    public class AuthPageModel : PageModel
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger<AuthPageModel> _logger;
        protected readonly IHttpContextAccessor _contextAccessor;

        public AuthPageModel(IHttpClientFactory httpClientFactory, ILogger<AuthPageModel> logger, IHttpContextAccessor contextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        protected HttpClient GetClient()
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient();
                string cookieValue = _contextAccessor.HttpContext.Request.Cookies["auth_token"];

                if (string.IsNullOrEmpty(cookieValue))
                {
                    _logger.LogWarning("JWT token cookie is null or empty.");
                    return null;
                }

                dynamic tokenObject = JsonConvert.DeserializeObject(cookieValue);
                string jwtToken = tokenObject.token;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get JWT token: {ex.Message}");
                return null;
            }
        }
        protected IActionResult EnsureAuthenticated()
        {
            if (!_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is not authenticated. Redirecting to login page.");
                return RedirectToPage("/Login");
            }
            else
            {
                _logger.LogInformation("User is authenticated.");
                return null;
            }
        }


    }
}
