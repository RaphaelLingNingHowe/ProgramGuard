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

        public LoginDto loginDto { get; set; }
        public async Task<IActionResult> OnPostAsync([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/Auth/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (jsonResponse.Trim().StartsWith("{") && jsonResponse.Trim().EndsWith("}"))
                    {
                        var responseObject = JsonConvert.DeserializeObject<RequirePasswordChangeDto>(jsonResponse);
                        if (responseObject.RequirePasswordChange)
                        {
                            return new JsonResult(new { requirePasswordChange = true, message = "超過80天未更換密碼，請更換密碼", success = true });
                        }
                    }
                    else
                    {
                        Response.Cookies.Append("auth_token", jsonResponse, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        });
                        return new JsonResult(new { requirePasswordChange = false, message = "登錄成功", success = true });
                    }
                    return Page();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = errorContent, success = false });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { message = $"An error occurred: {ex.Message}", success = false });
            }
        }

    }
}

