using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBG.Models;
using WebBG.ViewModels;

namespace WebBG.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class AdminCartsController : Controller
    {
        private readonly WebBgContext _context;

        public AdminCartsController(WebBgContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminCarts
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.Carts.Include(b => b.User).Where(x => x.CartId.ToUpper().Contains(SearchString.ToUpper()));
                return View(searchedData);
            }

            Hashtable statuses =  new Hashtable();
            statuses.Add(1, "Chờ xác nhận");
            statuses.Add(2, "Đã xác nhận");
            statuses.Add(3, "Đang giao hàng");
            statuses.Add(4, "Đã nhận hàng");
            statuses.Add(0, "Đã huỷ");

            ViewBag.MyHashTable = statuses;


            var webBgContext = await _context.Carts.Include(c => c.User).OrderByDescending(c => c.DateCreated).ToListAsync();
            return View(webBgContext);
        }

        public async Task<IActionResult> Loc(int? Status)
        {
            var ordersQuery = await _context.Carts.Include(c => c.User).OrderByDescending(c => c.DateCreated).ToListAsync();

            if (Status != null)
            {
                ordersQuery = ordersQuery.Where(s => s.Status == Status).ToList();
            }

            Hashtable statuses = new Hashtable();
            statuses.Add(1, "Chờ xác nhận");
            statuses.Add(2, "Đã xác nhận");
            statuses.Add(3, "Đang giao hàng");
            statuses.Add(4, "Đã nhận hàng");
            statuses.Add(0, "Đã huỷ");

            ViewBag.MyHashTable = statuses;

            return View("Index", ordersQuery);
        }


        public async Task<IActionResult> CartDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cartDetails = await _context.CartDetails.Include(bg => bg.Product).Where(cd => cd.CartId == id).ToListAsync();
            var list = cartDetails;

            return PartialView("_CartDetails", cartDetails);
        }

        // GET: Admin/AdminCarts/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartId == id);

            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Admin/AdminCarts/Create
        public async Task<IActionResult> Create()
        {
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username");
            return View();
        }

        // POST: Admin/AdminCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cart cart)
        {
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminCarts/Index");
            }
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username");
            return View(cart);
        }

        // GET: Admin/AdminCarts/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username");
            return View(cart);
        }

      

        // POST: Admin/AdminCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Cart cart)
        {
            ModelState.Remove("User");
            if (id != cart.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminCarts/Index");
            }
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username");
            return View(cart);
        }

        // GET: Admin/AdminCarts/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .Include(c=>c.CartDetails)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Admin/AdminCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cart = await _context.Carts.Include(c => c.CartDetails).FirstOrDefaultAsync(c => c.CartId == id);

            if (cart == null)
            {
                return NotFound(); // Handle case where cart is not found
            }

            // Remove cart details first
            foreach (var cartdetail in cart.CartDetails.ToList())
            {
                _context.CartDetails.Remove(cartdetail);
            }

            // Then remove the cart itself
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminCarts/Index");
        }

        private bool CartExists(string id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }
    }
}
