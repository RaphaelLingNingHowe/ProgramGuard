﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Enums;
using ProgramGuard.Web.Model;
using System.ComponentModel.DataAnnotations;
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
                    List<FileListDto> fileList = await response.Content.ReadFromJsonAsync<List<FileListDto>>();
                    return new OkObjectResult(fileList);
                }


                return new ObjectResult($"Failed to fetch data from API. Status code:{response.StatusCode}.") { StatusCode = (int)response.StatusCode };


            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch data from API.");
                return new ObjectResult($"Failed to fetch data from API, reason:{ex.Message}.") { StatusCode = 500 };
            }
        }

        [RegularExpression(@"^(?:[a-zA-Z]:|\\)\\(?:[\w\-. \u4E00-\u9FFF]+\\)*[\w\-. \u4E00-\u9FFF]+([\w.])*$", ErrorMessage = "無效的文件路徑")]
        public string FilePath { get; set; }
        public async Task<IActionResult> OnPostFilePathAsync([FromBody] string FilePath)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                return BadRequest("請輸入檔案路徑");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!FileExists(FilePath))
            {
                ModelState.AddModelError("FilePath", "檔案路徑不存在");
                return BadRequest("檔案路徑不存在");
            }
            try
            {
                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(FilePath), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/FileList", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = successContent, success = true });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { message = errorContent, success = false });
                }
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

        public async Task<IActionResult> OnDeleteAsync(string key)
        {
            try
            {

                HttpClient client = GetClient();
                HttpResponseMessage response = await client.DeleteAsync($"/FileList/{key}");

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
