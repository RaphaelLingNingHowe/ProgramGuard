using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class LogoutModel : BasePageModel
    {
        public LogoutModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                HttpClient client = GetClient();
                HttpResponseMessage response = await client.GetAsync("/Auth/logout");

                if (response.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("auth_token");
                    return Redirect("/Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to send data to API.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}
