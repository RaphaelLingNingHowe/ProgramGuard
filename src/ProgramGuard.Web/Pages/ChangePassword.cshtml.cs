using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Dtos.User;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    [AllowAnonymous]
    public class ChangePasswordModel : BasePageModel
    {
        public ChangePasswordModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }

        [BindProperty]
        public ChangePasswordDto changePasswordDto { get; set; }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {

            HttpClient client = GetClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(changePasswordDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/User/change-password", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = errorContent;
                return Page();
            }
        }

    }
}
