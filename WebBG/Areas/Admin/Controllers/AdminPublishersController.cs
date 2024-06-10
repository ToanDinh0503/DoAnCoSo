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
    public class AdminPublishersController : Controller
    {
        private readonly WebBgContext _context;

        public AdminPublishersController(WebBgContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminPublishers
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.Publishers.Where(x => x.PublisherName.ToUpper().Contains(SearchString.ToUpper()));
                return View(searchedData);
            }
            return View(await _context.Publishers.ToListAsync());
        }

        // GET: Admin/AdminPublishers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.PublisherId == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // GET: Admin/AdminPublishers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminPublishers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publisher);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminPublishers/Index");
            }
            return View(publisher);
        }

        // GET: Admin/AdminPublishers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            return View(publisher);
        }

        // POST: Admin/AdminPublishers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Publisher publisher)
        {
            if (id != publisher.PublisherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publisher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherExists(publisher.PublisherId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminPublishers/Index");
            }
            return View(publisher);
        }

        // GET: Admin/AdminPublishers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.PublisherId == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // POST: Admin/AdminPublishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher != null)
            {
                _context.Publishers.Remove(publisher);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminPublishers/Index");
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.PublisherId == id);
        }
    }
}
