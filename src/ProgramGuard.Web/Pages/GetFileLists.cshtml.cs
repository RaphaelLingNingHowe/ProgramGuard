using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Models;

namespace ProgramGuard_Web.Pages {
    public class GetFileListsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GetFileListsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<FileListDto> DataList { get; set; }

        public async Task OnGet()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7053/api/Files");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                DataList = JsonConvert.DeserializeObject<List<FileListDto>>(responseData);
            }
            else
            {
                // Handle error here
                // For example: ViewData["Error"] = "Failed to fetch data from API.";
            }
        }
    }
}
