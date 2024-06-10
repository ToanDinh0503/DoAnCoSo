using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBG.Models;

namespace WebBG.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class AdminFeedbacksController : Controller
    {
        private readonly WebBgContext _context;

        public AdminFeedbacksController(WebBgContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminFeedbacks
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.Feedbacks.Include(f => f.BoardGame).Include(f => f.User).Where(x => x.BoardGame.Name.ToUpper().Contains(SearchString.ToUpper()));
                return View(searchedData);
            }
            var webBgContext = _context.Feedbacks.Include(f => f.BoardGame).Include(f => f.User);
            return View(await webBgContext.ToListAsync());
        }

        // GET: Admin/AdminFeedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.BoardGame)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Admin/AdminFeedbacks/Create
        public async Task<IActionResult> Create()
        {

            var boardgames = await _context.BoardGames.ToListAsync();
            ViewBag.BoardGames = new SelectList(boardgames, "BoardGameId", "Name");
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username");
            return View();
        }

        // POST: Admin/AdminFeedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            ModelState.Remove("User");
            ModelState.Remove("BoardGame");
            if (ModelState.IsValid)
            {
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminFeedbacks/Index");
            }
            var boardgames = await _context.BoardGames.ToListAsync();
            ViewBag.BoardGames = new SelectList(boardgames, "BoardGameId", "Name",feedback.BoardGameId);
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username", feedback.UserId);
            return View(feedback);
        }

        // GET: Admin/AdminFeedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            var boardgames = await _context.BoardGames.ToListAsync();
            ViewBag.BoardGames = new SelectList(boardgames, "BoardGameId", "Name", feedback.BoardGameId);
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username", feedback.UserId);
            return View(feedback);
        }

        // POST: Admin/AdminFeedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Feedback feedback)
        {
            ModelState.Remove("User");
            ModelState.Remove("BoardGame");
            if (id != feedback.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.FeedbackId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminFeedbacks/Index");
            }
            var boardgames = await _context.BoardGames.ToListAsync();
            ViewBag.BoardGames = new SelectList(boardgames, "BoardGameId", "Name", feedback.BoardGameId);
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username", feedback.UserId);
            return View(feedback);
        }

        // GET: Admin/AdminFeedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.BoardGame)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Admin/AdminFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminFeedbacks/Index");
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackId == id);
        }
    }
}
