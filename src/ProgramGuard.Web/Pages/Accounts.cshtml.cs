using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    public class AccountsModel : BasePageModel
    {
        public AccountsModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }
        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }
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
        [BindProperty]
        public CreateUserDto createUserDto { get; set; }
        public async Task<IActionResult> OnPostAsync(string values)
        {
            ModelState.Clear();
            try
            {
                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(createUserDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/Account/addUser", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "文件创建成功！";
                    return Page();
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
