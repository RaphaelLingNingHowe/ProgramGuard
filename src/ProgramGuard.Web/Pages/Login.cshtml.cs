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
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(jsonResponse);
                    if (loginResponse == null)
                    {
                        return StatusCode((int)500, new { message = "伺服器異常，請稍後再試" });
                    }
                    if (loginResponse.RequirePasswordChange)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, new { requirePasswordChange = true, message = loginResponse.Message});

                    }
                    if (!string.IsNullOrEmpty(loginResponse.Token))
                    {
                        Response.Cookies.Append("auth_token", loginResponse.Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        });
                    return await HandleResponseAsync(response);

                    }
                    return await HandleResponseAsync(response);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorContent });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { message = $"An error occurred: {ex.Message}", success = false });
            }
        }
    }
}


