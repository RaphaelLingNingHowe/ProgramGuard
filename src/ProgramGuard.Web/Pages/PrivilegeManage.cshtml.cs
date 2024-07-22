using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Dtos.PrivilegeRule;
using ProgramGuard.Enums;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    public class PrivilegeManageModel : BasePageModel
    {
        public PrivilegeManageModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }
        public async Task<IActionResult> OnGet()
        {
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_ACCOUNT_PRIVILEGE) == false)
            {
                await LogActionAsync(ACTION.ACCESS_MANAGER_PAGE, "嘗試進入無權限頁面");
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnGetPrivilegeRuleAsync()
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync("/PrivilegeRule");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Received JSON: {jsonString}");
                    return new OkObjectResult(jsonString);
                }


                return new ObjectResult($"Failed to fetch data from API. Status code:{response.StatusCode}.") { StatusCode = (int)response.StatusCode };


            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch data from API.");
                return new ObjectResult($"Failed to fetch data from API, reason:{ex.Message}.") { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnGetPrivilegeAsync(string? contains)
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync("/Privilege");

                if (response.IsSuccessStatusCode)
                {
                    var privilegeRules = await response.Content.ReadFromJsonAsync<List<GetPrivilegeRuleDto>>();
                    return new OkObjectResult(privilegeRules);
                }
                else
                {
                    return new ObjectResult($"Failed to fetch data from API. Status code: {response.StatusCode}.")
                    {
                        StatusCode = (int)response.StatusCode
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch data from API.");
                return new ObjectResult($"Failed to fetch data from API, reason: {ex.Message}.")
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([FromBody] CreatePrivilegeDto createPrivilegeDto)
        {
            try
            {
                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(createPrivilegeDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/Privilege", jsonContent);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPutAsync(int key, [FromBody] UpdatePrivilegeDto updatePrivilegeDto)
        {
            try
            {
                HttpClient client = GetClient();

                var jsonContent = new StringContent(JsonConvert.SerializeObject(updatePrivilegeDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"/Privilege/{key}", jsonContent);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }
        public async Task<IActionResult> OnDeleteAsync(string key)
        {
            try
            {

                HttpClient client = GetClient();

                HttpResponseMessage response = await client.DeleteAsync($"/Privilege/{key}");

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
