using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Models;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages
{
    public class ActionLogModel : AuthPageModel
    {
        public ActionLogModel(IHttpClientFactory httpClientFactory, ILogger<AuthPageModel> logger, IHttpContextAccessor contextAccessor)
            : base(httpClientFactory, logger, contextAccessor)
        {

        }

        [BindProperty]
        public List<ActionLogDto> actionLog { get; set; }
        public async Task<IActionResult> OnGetData()
        {
            try
            {
                HttpClient client = GetClient();

                if (client == null)
                {
                    return RedirectToPage("/Login");
                }

                HttpResponseMessage response = await client.GetAsync("https://localhost:7053/api/ActionLog");

                if (response.IsSuccessStatusCode)
                {
                    List<ActionLogDto> fileList = await response.Content.ReadFromJsonAsync<List<ActionLogDto>>();
                    return new OkObjectResult(fileList);
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
