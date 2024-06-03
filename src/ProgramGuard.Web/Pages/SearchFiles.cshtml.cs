using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Models;
using System.Text;

namespace ProgramGuard.Web.Pages
{
    public class SearchFilesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SearchFilesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            searchDto = new SearchDto(); // 初始化 SearchDto
        }
        [BindProperty]
        public SearchDto searchDto{ get; set; }
        public List<SearchResultDto> SearchResults { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // 打印 SearchDto 的内容以确认它包含了所有期望的属性
            Console.WriteLine("SearchDto: " + JsonConvert.SerializeObject(searchDto));
            var client = _httpClientFactory.CreateClient();
            // 檢查是否至少填寫了一個搜索條件
            //if (string.IsNullOrWhiteSpace(SearchDto.FileName) && SearchDto.StartTime == null && SearchDto.EndTime == null)
            //{
            //    ModelState.AddModelError(string.Empty, "Please fill in at least one search criteria.");
            //    return Page(); // 返回頁面以重新顯示搜索表單並顯示錯誤消息
            //}
            if (string.IsNullOrWhiteSpace(searchDto.FileName) && (searchDto.StartTime == null && searchDto.EndTime == null))
            {
                ModelState.AddModelError(string.Empty, "Please fill in at least one search criteria.");
                return Page(); // 返回頁面以重新顯示搜索表單並顯示錯誤消息
            }
            var searchContent = new StringContent(JsonConvert.SerializeObject(searchDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7053/api/logquery/search", searchContent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                SearchResults = JsonConvert.DeserializeObject<List<SearchResultDto>>(responseData);
                return Page(); // 返回 Page() 以重新呈現頁面並顯示搜索結果
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page(); // 返回 Page() 以重新呈現頁面並顯示錯誤信息
            }
        }
    }
}
