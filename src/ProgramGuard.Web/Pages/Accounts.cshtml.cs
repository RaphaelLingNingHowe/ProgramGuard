using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages
{
    [Authorize]
    public class AccountsModel : BasePageModel
    {
        public AccountsModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

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
