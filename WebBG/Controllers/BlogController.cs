using Microsoft.AspNetCore.Mvc;
using WebBG.Models;
using WebBG.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace WebBG.Controllers
{
    public class BlogController : Controller
    {
        private readonly WebBgContext _context;
        public BlogController(WebBgContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int BlogPerPage = 2;
            var totalCount = await _context.Blogs.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)BlogPerPage);

            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blo = await _context.Blogs
                                        .OrderBy(p => p.BlogId)
                                        .Skip((page - 1) * BlogPerPage)
                                        .Take(BlogPerPage)
                                        .ToListAsync();
            var viewModel = new BlogViewModel
            {
                Menus = menus,
                Blogs = blogs,
                Bl = blo,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(viewModel);
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        public async Task<IActionResult> BlogDetail(int id)
        {
            // Lấy danh sách menu
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();

            // Lấy thông tin chi tiết của bài viết dựa trên ID
            var blog = await _context.Blogs.FindAsync(id);

            // Kiểm tra xem bài viết có tồn tại không
            if (blog == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = "Blog Error",
                };
                return View("Error", errorViewModel);
            }

            // Lấy danh sách bình luận cho bài viết
            var comments = await _context.Blogs.Where(c => c.BlogId == id).ToListAsync();

            var viewModel = new BlogViewModel
            {
                Menus = menus,
                Blogs = new List<Blog> { blog } // Tạo danh sách chỉ chứa bài viết hiện tại
            };
            return View(viewModel);
        }

    }
}
