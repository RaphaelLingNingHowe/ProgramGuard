using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Dtos.FileDetection;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    public class FileListsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FileListsModel> _logger;

        public FileListsModel(IHttpClientFactory httpClientFactory, ILogger<FileListsModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public string FilePath { get; set; }
        [BindProperty]
        public List<FileListDto> fileList { get; set; }

        public async Task OnGet()
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync("https://localhost:7053/api/Files");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    fileList = JsonConvert.DeserializeObject<List<FileListDto>>(responseData);
                }
                else
                {
                    LogError(response);
                    ViewData["Error"] = $"Failed to fetch data from API. Status code: {response.StatusCode}";
                }
            }
            catch (HttpRequestException ex)
            {
                LogException(ex);
                ViewData["Error"] = "There was a problem accessing the API.";
            }
            catch (Exception ex)
            {
                LogException(ex);
                ViewData["Error"] = "An unexpected error occurred.";
            }
        }

        private void LogError(HttpResponseMessage response)
        {
            var logMessage = $"Error fetching data from API. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}";
            _logger.LogError(logMessage);
        }

        private void LogException(Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                ModelState.AddModelError("FilePath", "File path is required.");
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(FilePath), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://localhost:7053/api/Files", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "文件创建成功！";
                    return RedirectToPage("/filelists");
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

        public async Task<IActionResult> OnPutAsync([FromBody] FileListDto fileList)
        {
            try
            {
                if (fileList == null)
                {
                    return new JsonResult(new { success = false, error = "Invalid data provided for update." });
                }

                var client = _httpClientFactory.CreateClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(fileList), Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"https://localhost:7053/api/Files/{fileList.Id}", jsonContent);

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

        public async Task<IActionResult> OnDeleteAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync($"https://localhost:7053/api/Files/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true });
                }
                else
                {
                    return new JsonResult(new { success = false, error = "Failed to delete resource." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
