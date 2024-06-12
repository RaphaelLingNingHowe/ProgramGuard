using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ProgramGuard.Web.Model
{
    public class BasePageModel : PageModel
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger<BasePageModel> _logger;
        protected readonly IHttpContextAccessor _contextAccessor;
        protected readonly IConfiguration _configuration;

        public BasePageModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }
        protected bool CheckTokenValidity()
        {
            string cookieValue = _contextAccessor.HttpContext.Request.Cookies["auth_token"];

            if (string.IsNullOrEmpty(cookieValue))
            {
                _logger.LogWarning("JWT token cookie is null or empty.");
                return false;
            }
            return true;
        }
        protected HttpClient GetClient()
        {
            string cookieValue = _contextAccessor.HttpContext.Request.Cookies["auth_token"];
            var client = _httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(cookieValue))
            {
                try
                {
                    var tokenObject = JsonConvert.DeserializeAnonymousType(cookieValue, new { token = "" });
                    string jwtToken = tokenObject.token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to deserialize JWT token: {ex.Message}");
                }
            }
            else
            {
                _logger.LogWarning("JWT token cookie is null or empty.");
            }

            string baseUrl = _configuration.GetValue<string>("ApiEndpoint:BaseUrl");
            if (!string.IsNullOrEmpty(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
            else
            {
                _logger.LogWarning("Base URL is not configured.");
            }

            return client;
        }

    }
}
