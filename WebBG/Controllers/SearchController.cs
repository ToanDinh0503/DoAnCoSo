using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBG.Models;
using WebBG.ViewModels;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace WebBG.Controllers
{
    public class SearchController : Controller
    {
        private readonly WebBgContext _context;
        public SearchController(WebBgContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> SearchPro(string searchTerm="")
        {
            var viewModel = new SearchViewModel();
            // Lấy danh sách menus và blogs
            viewModel.Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            viewModel.Blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Truy vấn cơ sở dữ liệu để tìm kiếm sản phẩm
                var products = await _context.BoardGames
                    .Where(p => p.Name.Contains(searchTerm))
                    .ToListAsync();
                viewModel.Bg = products;
                ViewBag.SearchTerm = searchTerm;
                return View("SearchPro", viewModel);
            }
            return View("SearchPro", viewModel);
        }
    }
}
