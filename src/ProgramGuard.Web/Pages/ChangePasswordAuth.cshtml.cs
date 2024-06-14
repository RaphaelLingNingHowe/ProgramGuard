using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Dtos.User;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    public class ChangePasswordAuthModel : BasePageModel
    {
        public ChangePasswordAuthModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }

        [BindProperty]
        public ChangePasswordDto changePasswordDto { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HttpClient client = GetClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(changePasswordDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/User/change-password", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "�K�X��令�\�I";
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
