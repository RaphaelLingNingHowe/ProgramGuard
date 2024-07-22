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

        public async Task<IActionResult> OnGetActionLogAsync(string startTime, string endTime)
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync($"/ActionLog?startTime={startTime}&endTime={endTime}");

                if (response.IsSuccessStatusCode)
                {
                    await LogActionAsync(ACTION.ACCESS_ACTION_LOG_PAGE);
                    var actionLog = await response.Content.ReadFromJsonAsync<List<GetActionLogDto>>();
                    return new OkObjectResult(actionLog);
                }

                return await HandleResponseAsync(response);
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
