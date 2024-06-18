using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages
{
    public class ActionLogsModel : BasePageModel
    {
        public ActionLogsModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
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
        public async Task<IActionResult> OnGetDataAsync()
        {
            try
            {
                HttpClient client = GetClient();


                HttpResponseMessage response = await client.GetAsync("/ActionLog");

                if (response.IsSuccessStatusCode)
                {
                    List<ActionLogDto> actionLogs = await response.Content.ReadFromJsonAsync<List<ActionLogDto>>();
                    return new OkObjectResult(actionLogs);
                }

                return new ObjectResult($"Failed to fetch data from API. Status code:{response.StatusCode}.") { StatusCode = (int)response.StatusCode };
            }
            catch (HttpRequestException ex)
            {
                return new ObjectResult($"Failed to fetch data from API, reason:{ex.Message}.") { StatusCode = 500 };
            }
        }
    }
}
