using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages
{
    [AllowAnonymous]
    public class LogoutModel : BasePageModel
    {
        public LogoutModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                HttpClient client = GetClient();
                HttpResponseMessage response = await client.PostAsync("/Auth/logout", null);

                if (response.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("auth_token");
                    return RedirectToPage("/Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to send data to API. Status code: {response.StatusCode}");
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
