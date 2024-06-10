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
    public class AdminBlogsController : Controller
    {
        private readonly WebBgContext _context;

        public AdminBlogsController(WebBgContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminBlogs
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.Blogs.Include(b => b.User).Where(x => x.Title.ToUpper().Contains(SearchString.ToUpper()));
                return View(searchedData);
            }
            var webBgContext = _context.Blogs.Include(b => b.User);
            return View(await webBgContext.ToListAsync());
        }

        // GET: Admin/AdminBlogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Admin/AdminBlogs/Create
        public async Task<IActionResult> Create()
        {
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username");
            
            return View();
        }

        // POST: Admin/AdminBlogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(Blog blog,IFormFile image)
        {
            
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    blog.Image = await SaveImage(image);
                }
                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                return Redirect("~/Admin/AdminBlogs/Index");
            }
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username");
            return View(blog);
        }

        // GET: Admin/AdminBlogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username",blog.UserId);
            return View(blog);
        }

        // POST: Admin/AdminBlogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blog blog, IFormFile image)
        {
            ModelState.Remove("image");
            if (id != blog.BlogId)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingBlog = await _context.Blogs.FindAsync(id);  
                    //neu hinh anh ban null thi gan no bang hinh anh cu
                    if (image == null)
                    {
                        blog.Image = existingBlog.Image;
                    }
                    else
                    {
                        // Lưu hình ảnh mới
                        blog.Image = await SaveImage(image);
                    }
                    existingBlog.Title = blog.Title;
                    existingBlog.Content = blog.Content;
                    existingBlog.Image = blog.Image;
                    existingBlog.Datebegin = blog.Datebegin;
                    existingBlog.UserId = blog.UserId;
                    existingBlog.Link = blog.Link;
                    existingBlog.OrderIndex = blog.OrderIndex;
                    existingBlog.Hidden = blog.Hidden;
                    _context.Blogs.Update(existingBlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.BlogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminBlogs/Index");
            }
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "Username");
            return View(blog);
        }

        // GET: Admin/AdminBlogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Admin/AdminBlogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminBlogs/Index");
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }

        //Ham luu hinh anh
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
    }
}
