using Microsoft.AspNetCore.Mvc;
using WebBG.Models;
using WebBG.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace WebBG.Controllers
{
    public class ContactController : Controller
    {
        private readonly WebBgContext _context;
        public ContactController(WebBgContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m =>m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m =>m.OrderIndex).Take(2).ToListAsync();
            var viewModel = new ContactViewModel
            {
                Menus = menus,
                Blogs = blogs,
            };
            return View(viewModel);
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _BlogPartial()
        {
            return PartialView();
        }
    }
}
