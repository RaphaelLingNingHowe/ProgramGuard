using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Enums;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages
{
    [Authorize]
    public class ChangeLogsModel : BasePageModel
    {
        public ChangeLogsModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }
        public async Task<IActionResult> OnGet()
        {
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_CHANGE_LOG) == false)
            {
                await LogActionAsync(ACTION.ACCESS_CHANGE_LOG_PAGE, "嘗試進入無權限頁面");
                return RedirectToPage("/Login");
            }
            return Page();
        }


        public async Task<IActionResult> OnGetChangeLogAsync(DateTime? startTime, DateTime? endTime, string fileName, bool? unConfirmed)
        {
            try
            {
                HttpClient client = GetClient();

                var url = "/ChangeLog";
                var queryParams = new List<string>();

                if (startTime.HasValue)
                {
                    queryParams.Add($"startTime={startTime.Value.ToString("o")}");
                }
                if (endTime.HasValue)
                {
                    queryParams.Add($"endTime={endTime.Value.ToString("o")}");
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    queryParams.Add($"fileName={Uri.EscapeDataString(fileName)}");
                }
                if (unConfirmed.HasValue)
                {
                    queryParams.Add($"unConfirmed={unConfirmed.Value}");
                }

                if (queryParams.Count > 0)
                {
                    url += "?" + string.Join("&", queryParams);
                }

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    await LogActionAsync(Enums.ACTION.ACCESS_CHANGE_LOG_PAGE);
                    var changeLog = await response.Content.ReadFromJsonAsync<List<GetChangeLogDto>>();
                    return new OkObjectResult(changeLog);
                }

                return new ObjectResult($"Failed to fetch data from API. Status code: {response.StatusCode}.")
                {
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                return new ObjectResult($"Failed to fetch data from API, reason: {ex.Message}.")
                {
                    StatusCode = 500
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult($"An error occurred: {ex.Message}")
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> OnPutConfirmAsync(string key)
        {
            try
            {

                HttpClient client = GetClient();

                HttpResponseMessage response = await client.PutAsync($"/ChangeLog/confirm/{key}", null);



                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true });
                }
                else
                {
                    return new JsonResult(new { success = false, error = "Failed to update resource." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
