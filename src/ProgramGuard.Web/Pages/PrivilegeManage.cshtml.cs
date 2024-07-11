using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Dtos.PrivilegeRule;
using ProgramGuard.Enums;
using ProgramGuard.Web.Model;
using System.Collections.Generic;
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
                await LogActionAsync(ACTION.ACCESS_MANAGER_PAGE, "���նi�J�L�v������");
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnGetDataAsync()
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync("/Privileges");

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

        public async Task<IActionResult> OnGetPrivilegeRuleAsync(string? contains)
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync("/Privilege");

                if (response.IsSuccessStatusCode)
                {
                    List<GetPrivilegeRuleDto> privilegeRules = await response.Content.ReadFromJsonAsync<List<GetPrivilegeRuleDto>>();
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

        public CreatePrivilegeDto createPrivilegeDto { get; set; }
        public async Task<IActionResult> OnPostAsync([FromBody] CreatePrivilegeDto createPrivilegeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(createPrivilegeDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/Privilege", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = successContent, success = true });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = errorContent, success = false });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPutAsync(int id, UpdatePrivilegeDto updatePrivilegeDto)
        {
            try
            {
                HttpClient client = GetClient();

                var jsonContent = new StringContent(JsonConvert.SerializeObject(updatePrivilegeDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"/Privilege/{id}", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = successContent, success = true });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = errorContent, success = false });
                }
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

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = successContent, success = true });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = errorContent, success = false });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }
    }
}