using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    public class ChangeLogsModel : BasePageModel
    {
        public ChangeLogsModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
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

                HttpResponseMessage response = await client.GetAsync("/ChangeLog");

                if (response.IsSuccessStatusCode)
                {
                    List<GetChangeLogDto> changeLog = await response.Content.ReadFromJsonAsync<List<GetChangeLogDto>>();
                    return new OkObjectResult(changeLog);
                }

                return new ObjectResult($"Failed to fetch data from API. Status code:{response.StatusCode}.") { StatusCode = (int)response.StatusCode };
            }
            catch (HttpRequestException ex)
            {
                return new ObjectResult($"Failed to fetch data from API, reason:{ex.Message}.") { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new ObjectResult($"An error occurred: {ex.Message}") { StatusCode = 500 };
            }
        }



        public async Task<IActionResult> OnPutAsync(string key, string values)
        {
            try
            {

                HttpClient client = GetClient();

                StringContent jsonContent = new StringContent(values, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"/ChangeLog/confirm/{key}", jsonContent);



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
