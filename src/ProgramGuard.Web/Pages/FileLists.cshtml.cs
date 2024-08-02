using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Enums;
using ProgramGuard.Web.Model;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    [Authorize]
    public class FileListsModel : BasePageModel
    {
        public FileListsModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {

        }
        public async Task<IActionResult> OnGet()
        {
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_FILELIST) == false)
            {
                await LogActionAsync(ACTION.ACCESS_FILELIST_PAGE, "嘗試進入無權限頁面");
                return RedirectToPage("/Login");
            }
            return Page();
        }



        public async Task<IActionResult> OnGetData()
        {
            try
            {
                HttpClient client = GetClient();

                HttpResponseMessage response = await client.GetAsync("/FileList");

                if (response.IsSuccessStatusCode)
                {
                    await LogActionAsync(ACTION.ACCESS_FILELIST_PAGE);
                    List<FileListDto>? fileList = await response.Content.ReadFromJsonAsync<List<FileListDto>>();
                    return new OkObjectResult(fileList);
                }
                return await HandleResponseAsync(response);
            }
            catch (HttpRequestException ex)
            {
                return new ObjectResult($"Failed to fetch data from API, reason:{ex.Message}.") { StatusCode = 500 };
            }
        }

        public CreateFileDto createFileDto { get; set; } = new CreateFileDto();
        public async Task<IActionResult> OnPostCreateFileAsync([FromBody] CreateFileDto createFileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("驗證失敗，請檢查輸入的格式");
            }
            if (!FileExists(createFileDto.FilePath))
            {
                return BadRequest("檔案路徑不存在");
            }
            try
            {
                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(createFileDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/FileList", jsonContent);

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }

        private bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public async Task<IActionResult> OnPutAsync(string key, string values)
        {
            try
            {

                HttpClient client = GetClient();

                StringContent jsonContent = new StringContent(values, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"FileList/{key}", jsonContent);

                return await HandleResponseAsync(response);
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
                HttpResponseMessage response = await client.DeleteAsync($"/FileList/{key}");

                return await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
