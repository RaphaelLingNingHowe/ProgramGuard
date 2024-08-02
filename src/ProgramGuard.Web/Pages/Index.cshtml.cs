using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Web.Model;

namespace ProgramGuard.Web.Pages
{
    public class IndexModel : BasePageModel
    {
        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IHttpContextAccessor contextAccessor, IConfiguration configuration)
            : base(httpClientFactory, logger, contextAccessor, configuration)
        {
        }

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/FileLists");
            }
            else
            {
                return RedirectToPage("/Login");
            }

        }
    }
}
