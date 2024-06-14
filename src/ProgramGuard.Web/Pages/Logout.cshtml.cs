using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
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

                HttpResponseMessage response = await client.PostAsync("/User/logout",null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "µn¥X¦¨¥\¡I";
                    return RedirectToPage("/Login");
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
