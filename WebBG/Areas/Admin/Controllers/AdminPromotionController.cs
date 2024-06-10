using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBG.Models;
using System;

namespace WebBG.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class AdminPromotionController : Controller
    {
       

        private readonly WebBgContext _context;

        public AdminPromotionController(WebBgContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var promo = _context.Promotion.Where(x => x.PromoCode.ToUpper().Contains(SearchString.ToUpper()));
                return View(promo);
            }
            var webBgContext = _context.Promotion;
            return View(await webBgContext.ToListAsync());
        }



        // GET: AdminPromotionController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promo = await _context.Promotion
                .FirstOrDefaultAsync(m => m.PromoId == id);
            if (promo == null)
            {
                return NotFound();
            }

            return View(promo);
        }

        // GET: AdminPromotionController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminPromotionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Promotion promo)
        {
            var existPromo = _context.Promotion.Any(e => e.PromoCode == promo.PromoCode);

            if(existPromo)
            {
                ModelState.AddModelError("PromoCode", "Mã khuyến mãi đã tồn tại.");
                return View();
            }

            if (ModelState.IsValid)
            {
                _context.Promotion.Add(promo);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminPromotion/Index");
            }

            return View(promo);
        }

        // GET: AdminPromotionController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promo = await _context.Promotion.FindAsync(id);
            if (promo == null)
            {
                return NotFound();
            }

           
            return View(promo);
        }

        // POST: AdminPromotionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Promotion promo) { 
        
            if (id != promo.PromoId)
            {
                return NotFound();
            }


            if (PromotionCodeExists(id, promo.PromoCode))
            {
                ModelState.AddModelError("PromoCode", "Mã khuyến mãi đã tồn tại.");
                return View(promo);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPromo = await _context.Promotion.FindAsync(id); 
                    
                    existingPromo.PromoCode = promo.PromoCode;
                    existingPromo.DiscountValue = promo.DiscountValue;
                    existingPromo.DiscountPercent = promo.DiscountPercent;
                    existingPromo.ExpirationDate = promo.ExpirationDate;
                   

                    _context.Promotion.Update(existingPromo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promo.PromoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminPromotion/Index");
            }
            return View(promo);
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotion.Any(e => e.PromoId == id);
        }

        private bool PromotionCodeExists(int? id,string promoCode)
        {
            return _context.Promotion.Any(e => e.PromoId != id && e.PromoCode == promoCode);
        }

        // GET: AdminPromotionController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promo = await _context.Promotion.FirstOrDefaultAsync(m => m.PromoId == id);
            if (promo == null)
            {
                return NotFound();
            }

            return View(promo);
        }

        // POST: AdminPromotionController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promo = await _context.Promotion.FindAsync(id);
            if (promo != null)
            {
                _context.Promotion.Remove(promo);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminPromotion/Index");
        }
    }
}
