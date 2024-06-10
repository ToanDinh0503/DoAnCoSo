using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBG.Models;
using WebBG.ViewModels;

namespace WebBG.Controllers
{
    public class FeedBackController : Controller
    {
        private readonly WebBgContext _context;
        public FeedBackController(WebBgContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> _MenuPartial()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            return PartialView(menus);
        }
        public async Task<IActionResult> AddFB(int boardGameId)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();

            var viewModel = new FeedbackViewModel
            {
                Menus = menus,
                BoardGameId = boardGameId,
                Fb = new Feedback { Datebegin = DateTime.Now }
            };
            viewModel.Fb.Datebegin = DateTime.Now;
            var userName = User.Identity.Name;
            ViewBag.UserId = _context.Feedbacks.FirstOrDefault(x=>x.User.FullName==userName).UserId;
            // Lấy thông tin về trò chơi từ context
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFB(FeedbackViewModel fbViewModel)
        {
            ModelState.Remove("User");
            ModelState.Remove("FB.User");
            ModelState.Remove("FB.BoardGame");
            ModelState.Remove("Bg");
            if (ModelState.IsValid)
            {
                var feedback = new Feedback
                {
                    UserId = fbViewModel.Fb.UserId,
                    BoardGameId = fbViewModel.Fb.BoardGameId,
                    Content = fbViewModel.Fb.Content,
                    Rating = fbViewModel.Fb.Rating,
                    Datebegin = fbViewModel.Fb.Datebegin
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
            }
            return Redirect("/san-pham");
        }
    }
}
