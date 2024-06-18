using Microsoft.AspNetCore.Mvc.RazorPages;

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
        protected HttpClient GetClient()
        {
            string jwt = _contextAccessor.HttpContext.Request.Cookies["auth_token"];
            var client = _httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(jwt))
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
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
