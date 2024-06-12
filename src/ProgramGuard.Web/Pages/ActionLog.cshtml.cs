using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Models;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages
{
    public class ActionLogModel : BasePageModel
    {
        public ActionLogModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }
        public IActionResult OnGet()
        {
            var tokenValid = CheckTokenValidity(); // 检查令牌是否有效的自定义方法

            if (!tokenValid)
            {
                return RedirectToPage("/Login");
            }

            // 已认证用户的处理逻辑
            return Page();
        }

        [BindProperty]
        public List<ActionLogDto> actionLog { get; set; }
        public async Task<IActionResult> OnGetData()
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
