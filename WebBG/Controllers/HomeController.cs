using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebBG.Models;
using WebBG.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace WebBG.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebBgContext _context;
        public HomeController(WebBgContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var slides = await _context.Sliders.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            var viewModel = new HomeViewModel
            {
                Menus = menus,
                Blogs = blogs,
                Sliders = slides,
                CategoryProducts = new Dictionary<Category, List<BoardGame>>(),
                Categories = categories
            };

            // Lấy danh sách tất cả các danh mục có sẵn trong cơ sở dữ liệu
            

            // Duyệt qua từng danh mục để lấy sản phẩm
            foreach (var category in categories)
            {
                // Lấy tất cả sản phẩm thuộc danh mục hiện tại
                var products = await _context.BoardGames
                    .Where(m => m.Hidden == 0 && m.CategoryId == category.CategoryId)
                    .OrderBy(m => m.OrderIndex)
                    .ToListAsync();

                // Thêm danh sách sản phẩm vào viewModel
                viewModel.CategoryProducts.Add(category, products);
            }

            return View(viewModel);
        }
        public async Task<IActionResult> _SlidePartial()
        {
            return PartialView();
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        public async Task<IActionResult> _BlogPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _ProductPartial()
        {
            return PartialView();
        }

    }
}
