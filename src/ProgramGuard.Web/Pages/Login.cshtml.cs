using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Dtos.User;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    [AllowAnonymous]
    public class LoginModel : BasePageModel
    {
        public LoginModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }



        [BindProperty]
        public LoginDto LoginDto { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {


            // 使用基类方法获取 HttpClient 实例
            using (HttpClient client = GetClient())
            {
                if (client == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create HttpClient.");
                    return Page();
                }

                var loginContent = new StringContent(JsonConvert.SerializeObject(LoginDto), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/User/login", loginContent);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    Response.Cookies.Append("auth_token", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    return RedirectToPage("/FileLists");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
        }
    }
}
