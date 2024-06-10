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
    public class AdminPaymentsController : Controller
    {
        private readonly WebBgContext _context;

        public AdminPaymentsController(WebBgContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminPayments
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                if (DateTime.TryParse(SearchString, out DateTime searchDate))
                {
                    // Filter payments where the payment date matches the search date
                    var searchedData = _context.Payments
                        .Include(p => p.Cart)
                        .Where(p => p.PaymentDate.Date == searchDate.Date);

                    return View(await searchedData.ToListAsync());
                }
                else
                {
                    ModelState.AddModelError("SearchString", "Nhập sai định dạng nam/thang/ngay");
                    ViewBag.InvalidDateFormat = true; // Set a flag to indicate invalid date format
                    var data = _context.Payments.Include(p => p.Cart);
                    return View(await data.ToListAsync());
                }
            }
            var webBgContext = _context.Payments.Include(p => p.Cart);
            return View(await webBgContext.ToListAsync());
        }

        // GET: Admin/AdminPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Cart).ThenInclude(c => c.CartDetails).ThenInclude(e => e.Product)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Admin/AdminPayments/Create
        public async Task<IActionResult> Create()
        {
            var carts = await _context.Carts.ToListAsync();
            ViewBag.Carts = new SelectList(carts, "CartId", "CartId");
            return View();
        }

        // POST: Admin/AdminPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            ModelState.Remove("Cart");
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminPayments/Index");
            }
            var carts = await _context.Carts.ToListAsync();
            ViewBag.Carts = new SelectList(carts, "CartId", "CartId", payment.CartId);
            return View(payment);
        }

        // GET: Admin/AdminPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            var carts = await _context.Carts.ToListAsync();
            ViewBag.Carts = new SelectList(carts, "CartId", "CartId", payment.CartId);
            return View(payment);
        }

        // POST: Admin/AdminPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Payment payment)
        {
            ModelState.Remove("Cart");
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminPayments/Index");
            }
            var carts = await _context.Carts.ToListAsync();
            ViewBag.Carts = new SelectList(carts, "CartId", "CartId", payment.CartId);
            return View(payment);
        }

        // GET: Admin/AdminPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Cart)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Admin/AdminPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminPayments/Index");
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }
    }
}
