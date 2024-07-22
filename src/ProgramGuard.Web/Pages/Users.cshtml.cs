using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Dtos.PrivilegeRule;
using ProgramGuard.Dtos.User;
using ProgramGuard.Enums;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    [Authorize]
    public class UsersModel : BasePageModel
    {
        public UsersModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }
        public async Task<IActionResult> OnGet()
        {
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_ACCOUNT_MANAGER) == false)
            {
                await LogActionAsync(ACTION.ACCESS_MANAGER_PAGE, "嘗試進入無權限頁面");
                return RedirectToPage("/Login");
            }
            return Page();
        }
        public async Task<IActionResult> OnGetDataAsync()
        {

            try
            {

                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync("/User");

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

        public CreateUserDto createUserDto { get; set; }
        public async Task<IActionResult> OnPostCreateUserAsync([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "驗證失敗，請檢查輸入的格式", success = false });
                }

                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(createUserDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/User/createUser", jsonContent);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
        public async Task<IActionResult> OnPutActiveAccountAsync(string key)
        {
            try
            {

                HttpClient client = GetClient();

                HttpResponseMessage response = await client.PutAsync($"/User/{key}/Active", null);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPutDisableAccountAsync(string key)
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.PutAsync($"/User/{key}/Disable", null);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPutUnlockAccountAsync(string userId)
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.PutAsync($"/User/{userId}/unlock", null);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }

        public ResetPasswordDto resetPasswordDto { get; set; }
        public async Task<IActionResult> OnPostResetPasswordAsync(string key, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(resetPasswordDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"/User/{key}/ResetPassword", jsonContent);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnDeleteAsync(string key)
        {
            try
            {

                HttpClient client = GetClient();

                HttpResponseMessage response = await client.DeleteAsync($"/User/{key}");

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }
        public async Task<IActionResult> OnGetPrivilegeAsync()
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync("/Privilege");

                if (response.IsSuccessStatusCode)
                {
                    List<GetPrivilegeRuleDto> privilegeDto = await response.Content.ReadFromJsonAsync<List<GetPrivilegeRuleDto>>();
                    return new OkObjectResult(privilegeDto);
                }
                return await HandleResponseAsync(response);

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch data from API.");
                return new ObjectResult($"Failed to fetch data from API, reason:{ex.Message}.") { StatusCode = 500 };
            }
        }
        public async Task<IActionResult> OnPutUpdatePrivilegeAsync(string userId, int privilegeId)
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.PutAsync($"/User/{userId}/privileges/{privilegeId}", null);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }
    }
}

