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

        public ChangePasswordDto changePasswordDto { get; set; }
        public async Task<IActionResult> OnPutChangePasswordAsync(string key, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "驗證失敗，請檢查輸入的格式", success = false });
            }
            HttpClient client = GetClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(changePasswordDto), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/Auth/{key}/changePassword", jsonContent);


            return await HandleResponseAsync(response);
        }


    }
}
