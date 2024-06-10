using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebBG.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        [Authorize(Policy = "RequireAdminPermission")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult dangNhap()
        {
            return View();
        }
    }
}
