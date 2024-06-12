using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Web.Model;
using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Web.Pages
{
    public class AccountModel : BasePageModel
    {
        public AccountModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }
        public IActionResult OnGet()
        {
            var tokenValid = CheckTokenValidity(); // 检查令牌是否有效的自定义方法

            if (!tokenValid)
            {
                return RedirectToPage("/Login");
            }

            // 已认证用户的处理逻辑
            return Page();
        }

        public async Task<IActionResult> OnGetData()
        {

            try
            {

                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync("/Account");

                if (response.IsSuccessStatusCode)
                {
                    List<GetUserDto> userDtos = await response.Content.ReadFromJsonAsync<List<GetUserDto>>();
                    return new OkObjectResult(userDtos);
                }


                return new ObjectResult($"Failed to fetch data from API. Status code:{response.StatusCode}.") { StatusCode = (int)response.StatusCode };


            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch data from API.");
                return new ObjectResult($"Failed to fetch data from API, reason:{ex.Message}.") { StatusCode = 500 };
            }
        }
    }
}
