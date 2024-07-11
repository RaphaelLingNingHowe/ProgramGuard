using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Dtos.ActionLog;
using ProgramGuard.Enums;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages.Account
{
    [Authorize]
    public class ActionLogsModel : BasePageModel
    {
        public ActionLogsModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {
        }
        public async Task<IActionResult> OnGet()
        {
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_ACTION_LOG) == false)
            {
                await LogActionAsync(ACTION.ACCESS_ACTION_LOG_PAGE, "嘗試進入無權限頁面");
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnGetActionLogAsync(DateTime? startTime, DateTime? endTime)
        {
            try
            {
                HttpClient client = GetClient();

                var url = "/ActionLog";
                var queryParams = new List<string>();

                if (startTime.HasValue)
                {
                    queryParams.Add($"startTime={startTime.Value.ToString("o")}");
                }
                if (endTime.HasValue)
                {
                    queryParams.Add($"endTime={endTime.Value.ToString("o")}");
                }

                if (queryParams.Count > 0)
                {
                    url += "?" + string.Join("&", queryParams);
                }

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    await LogActionAsync(ACTION.ACCESS_ACTION_LOG_PAGE);
                    var actionLog = await response.Content.ReadFromJsonAsync<List<GetActionLogDto>>();
                    return new OkObjectResult(actionLog);
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
    }
}
