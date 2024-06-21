using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Dtos.User;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages.Account
{
    [Authorize(Roles = "Admin")]
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
        [BindProperty]
        public CreateUserDto createUserDto { get; set; }
        public async Task<IActionResult> OnPostAsync(string values)
        {
            try
            {
                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(createUserDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/User/addUser", jsonContent);

                if (response.IsSuccessStatusCode)
                {
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

        public async Task<IActionResult> OnPutAsync(string key, string values)
        {
            try
            {
                HttpClient client = GetClient();
                bool isAdminUpdated = false;
                bool isFrozenUpdated = false;

                // 解析 JSON 字串為 JObject
                JObject jsonObject = JObject.Parse(values);

                // 檢查是否包含 IsAdmin
                if (jsonObject.ContainsKey("IsAdmin"))
                {
                    bool isAdmin = (bool)jsonObject["IsAdmin"];
                    StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(new { IsAdmin = isAdmin }), Encoding.UTF8, "application/json");
                    HttpResponseMessage responseAdmin = await client.PutAsync($"/User/toggleAdmin/{key}", jsonContent);

                    if (responseAdmin.IsSuccessStatusCode)
                    {
                        isAdminUpdated = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to update IsAdmin.");
                    }
                }

                // 檢查是否包含 IsFrozen
                if (jsonObject.ContainsKey("IsFrozen"))
                {
                    bool isFrozen = (bool)jsonObject["IsFrozen"];
                    StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(new { IsFrozen = isFrozen }), Encoding.UTF8, "application/json");
                    HttpResponseMessage responseFrozen = await client.PutAsync($"/User/toggleFreeze/{key}", jsonContent);

                    if (responseFrozen.IsSuccessStatusCode)
                    {
                        isFrozenUpdated = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to update IsFrozen.");
                    }
                }

                // 檢查是否有任何操作成功
                if (isAdminUpdated || isFrozenUpdated)
                {
                    return new JsonResult(new { success = true });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No valid operations found."); // 找不到有效的操作
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}"); // 處理異常
                return Page();
            }
        }
        [BindProperty]
        public ChangePasswordDto changePasswordDto { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            HttpClient client = GetClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(changePasswordDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/User/change-password", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var successContent = await response.Content.ReadAsStringAsync();
                TempData["SuccessMessage"] = successContent;
                return Redirect("/Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = errorContent;
                return Page();
            }
        }

    }
}

