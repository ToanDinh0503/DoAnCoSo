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
    public class AdminCartDetailsController : Controller
    {
        private readonly WebBgContext _context;

        public AdminCartDetailsController(WebBgContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminCartDetails
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.CartDetails.Include(c => c.Product).Include(c => c.Cart).Where(x => x.CartId==SearchString);
                return View(searchedData);
            }
            var webBgContext = _context.CartDetails.Include(c => c.Product).Include(c => c.Cart);
            return View(await webBgContext.ToListAsync());
        }

        // GET: Admin/AdminCartDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartDetail = await _context.CartDetails
                .Include(c => c.Product)
                .Include(c => c.Cart)
                .FirstOrDefaultAsync(m => m.CartDetailId == id);
            if (cartDetail == null)
            {
                return NotFound();
            }

            return View(cartDetail);
        }

        // GET: Admin/AdminCartDetails/Create
        public async Task<IActionResult> Create()
        {
            var boardgames = await _context.BoardGames.ToListAsync();
            ViewBag.BoardGames = new SelectList(boardgames, "BoardGameId", "Name");
            var carts = await _context.Carts.ToListAsync();
            ViewBag.Carts = new SelectList(carts, "CartId", "CartId");
            return View();
        }

        // POST: Admin/AdminCartDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CartDetail cartDetail)
        {
            ModelState.Remove("Cart");
            ModelState.Remove("BoardGame");
            if (ModelState.IsValid)
            {
                _context.Add(cartDetail);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminCartDetails/Index");
            }
            var boardgames = await _context.BoardGames.ToListAsync();
            ViewBag.BoardGames = new SelectList(boardgames, "BoardGameId", "Name",cartDetail.BoardGameId);
            var carts = await _context.Carts.ToListAsync();
            ViewBag.Carts = new SelectList(carts, "CartId", "CartId", cartDetail.CartId);
            return View(cartDetail);
        }

        // GET: Admin/AdminCartDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartDetail = await _context.CartDetails.FindAsync(id);
            if (cartDetail == null)
            {
                return NotFound();
            }
            var boardgames = await _context.BoardGames.ToListAsync();
            ViewBag.BoardGames = new SelectList(boardgames, "BoardGameId", "Name", cartDetail.BoardGameId);
            var carts = await _context.Carts.ToListAsync();
            ViewBag.Carts = new SelectList(carts, "CartId", "CartId", cartDetail.CartId);
            return View(cartDetail);
        }

        // POST: Admin/AdminCartDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,CartDetail cartDetail)
        {
            ModelState.Remove("Cart");
            ModelState.Remove("BoardGame");
            if (id != cartDetail.CartDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartDetailExists(cartDetail.CartDetailId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminCartDetails/Index");
            }
            var boardgames = await _context.BoardGames.ToListAsync();
            ViewBag.BoardGames = new SelectList(boardgames, "BoardGameId", "Name", cartDetail.BoardGameId);
            var carts = await _context.Carts.ToListAsync();
            ViewBag.Carts = new SelectList(carts, "CartId", "CartId", cartDetail.CartId);
            return View(cartDetail);
        }

        // GET: Admin/AdminCartDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartDetail = await _context.CartDetails
                .Include(c => c.Product)
                .Include(c => c.Cart)
                .FirstOrDefaultAsync(m => m.CartDetailId == id);
            if (cartDetail == null)
            {
                return NotFound();
            }

            return View(cartDetail);
        }

        // POST: Admin/AdminCartDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartDetail = await _context.CartDetails.FindAsync(id);
            if (cartDetail != null)
            {
                _context.CartDetails.Remove(cartDetail);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminCartDetails/Index");
        }

        private bool CartDetailExists(int id)
        {
            return _context.CartDetails.Any(e => e.CartDetailId == id);
        }
    }
}
