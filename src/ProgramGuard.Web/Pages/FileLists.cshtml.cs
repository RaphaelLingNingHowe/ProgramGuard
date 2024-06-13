﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Models;

using ProgramGuard.Web.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;


using System.Text;
using System.Threading.Tasks;

namespace ProgramGuard.Web.Pages
{
    public class FileListsModel : AuthPageModel
    {
        public FileListsModel(IHttpClientFactory httpClientFactory, ILogger<AuthPageModel> logger, IHttpContextAccessor contextAccessor)
            : base(httpClientFactory, logger, contextAccessor)
        {

        }

        [BindProperty]
        [RegularExpression(@"^(?:[a-zA-Z]:|\\)\\(?:[\w\-. \u4E00-\u9FFF]+\\)*[\w\-. \u4E00-\u9FFF]+([\w.])*$", ErrorMessage = "Invalid file path.")]
        public string FilePath { get; set; }



        public async Task<IActionResult> OnGetData()
        {
            
            try
            {

                HttpClient client = GetClient();

                if (client == null)
                {
                    return RedirectToPage("/Login");
                }

                HttpResponseMessage response = await client.GetAsync("https://localhost:7053/api/Files");

                if (response.IsSuccessStatusCode)
                {
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





        public async Task<IActionResult> OnPostAsync(string values)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                ModelState.AddModelError("FilePath", "File path is required.");
                return Page();
            }
            // 检查文件是否存在
            if (!FileExists(FilePath))
            {
                ModelState.AddModelError("FilePath", "檔案路徑不存在");
                return Page();
            }
            try
            {
                HttpClient client = GetClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(FilePath), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://localhost:7053/api/Files", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "文件创建成功！";
                    return Page();
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
        // 添加一个自定义的方法来检查文件是否存在
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

                HttpResponseMessage response = await client.PutAsync($"https://localhost:7053/api/Files/{key}", jsonContent);
                


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
                HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7053/api/Files/{key}");
              


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
