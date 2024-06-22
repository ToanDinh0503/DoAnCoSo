using Microsoft.AspNetCore.Mvc;
using WebBG.Models;
using WebBG.ViewModels;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;

namespace WebBG.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebBgContext _context;
        public ProductController(WebBgContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _BlogPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> CateProd(string slug, long id)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var cateProds = await _context.Categories.Where(cp => cp.CategoryId == id && cp.Link == slug).FirstOrDefaultAsync();
            if (cateProds == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = "CateProd Error",
                };
                return View("Error", errorViewModel);
            }
            var prods = await _context.BoardGames.Where(m => m.Hidden == 0 && m.CategoryId == cateProds.CategoryId).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new ProductViewModel
            {
                Menus = menus,
                Blogs = blogs,
                Bg = prods,
                cateName = cateProds.CategoryName,
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ProdDetail(string slug, int id)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var prods = await _context.BoardGames
                .Include(bg => bg.Publisher) // Eager loading cho Publisher
                .Include(bg => bg.Category)
                .Where(m => m.Link == slug && m.BoardGameId == id)
                .ToListAsync();
            var listfb = await _context.Feedbacks
            .Where(f => f.BoardGameId == id)
            .OrderBy(m => m.BoardGameId)
            .Select(f => new Feedback
            {
                FeedbackId = f.FeedbackId,
                UserId = f.UserId,
                BoardGameId = f.BoardGameId,
                Content = f.Content,
                Rating = f.Rating,
                Datebegin = f.Datebegin,
                User = f.User,
                BoardGame = f.BoardGame
            })
            .ToListAsync();
            if (!prods.Any())
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = "Product Error",
                };
                return View("Error", errorViewModel);
            }

            var viewModel = new ProductViewModel
            {
                Menus = menus,
                Blogs = blogs,
                Bg = prods,
                Fb = listfb,
                BoardGameId = id
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Search(string searchTerm = "")
        {
            {
                // Lấy danh sách menus và blogs
                var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
                var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
                //lay categories
                var categories = await _context.Categories.ToListAsync();

                ViewBag.Categories = categories;
                // Nếu searchTerm không được chỉ định hoặc là chuỗi rỗng, trả về tất cả sản phẩm
                if (string.IsNullOrEmpty(searchTerm))
                {
                    var allProducts = await _context.BoardGames.ToListAsync();
                    var viewModel = new ProductViewModel
                    {
                        Menus = menus,
                        Blogs = blogs,
                        Bg = allProducts
                    };
                    return View("Index", viewModel);
                }

                // Truy vấn cơ sở dữ liệu để tìm kiếm sản phẩm
                var products = await _context.BoardGames
                    .Where(p => p.Name.Contains(searchTerm))
                    .ToListAsync();

                var searchViewModel = new ProductViewModel
                {
                    Menus = menus,
                    Blogs = blogs,
                    Bg = products
                };

                return View("Index", searchViewModel);
            }
        }
        public async Task<IActionResult> Loc(int? categoryId, decimal? maxPrice = null)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            //lay ra cac categories
            var categories = await _context.Categories.ToListAsync();

            ViewBag.Categories = categories;

            //lay ra cac boardgame
            IQueryable<BoardGame> productsQuery = _context.BoardGames;

            //loc theo category
            if (categoryId != null && categoryId != 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }
            //loc theo gia
            if (maxPrice != null && maxPrice != 0)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice);
            }

            var products = await productsQuery.ToListAsync();

            var viewModel = new ProductViewModel
            {
                Menus = menus,
                Blogs = blogs,
                Bg = products
            };

            return View("Index", viewModel);
        }
        public async Task<IActionResult> Index(int page = 1, int? categoryId = null, decimal? maxPrice = null)
        {
            int productsPerPage = 4;
            var totalCount = await _context.BoardGames.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)productsPerPage);

            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();

            IQueryable<BoardGame> productsQuery = _context.BoardGames;

            if (categoryId != null)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }
            if (maxPrice != null)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice);
            }


            var products = await productsQuery
                .OrderByDescending(p => p.Quantity)
                .Skip((page - 1) * productsPerPage)
                .Take(productsPerPage)
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();

            ViewBag.Categories = categories;

            var viewModel = new ProductViewModel
            {
                Menus = menus,
                Blogs = blogs,
                Bg = products,
                TotalPages = totalPages,
                CurrentPage = page
            };


            return View(viewModel);
        }
    }
}