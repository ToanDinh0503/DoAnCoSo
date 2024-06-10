using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
    public class AdminBoardGamesController : Controller
    {
        private readonly WebBgContext _context;

        public AdminBoardGamesController(WebBgContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminBoardGames
        public async Task<IActionResult> Index(string SearchString="")
        {
            if(SearchString != ""){
                var sanPham = _context.BoardGames.Include(b => b.Category).Include(b => b.Publisher).Where(x=>x.Name.ToUpper().Contains(SearchString.ToUpper()));
                return View(sanPham);
            }
            var webBgContext = _context.BoardGames.Include(b => b.Category).Include(b => b.Publisher);
            return View(await webBgContext.ToListAsync());
        }

        // GET: Admin/AdminBoardGames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.BoardGameId == id);
            if (boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

        // GET: Admin/AdminBoardGames/Create
        public IActionResult Create()
        {
            var categories =  _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var publishers = _context.Publishers.ToList();
            ViewBag.Publishers = new SelectList(publishers, "PublisherId", "PublisherName");
            return View();
        }

        // POST: Admin/AdminBoardGames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardGame boardGame, IFormFile image1, IFormFile image2, IFormFile image3)
        {
            if (ModelState.IsValid)
            {
                if (image1 != null)
                {
                    boardGame.Image1 = await SaveImage(image1);
                }
                if (image2 != null)
                {
                    boardGame.Image2 = await SaveImage(image2);
                }
                if (image3 != null)
                {
                    boardGame.Image3 = await SaveImage(image3);
                }
                _context.BoardGames.Add(boardGame);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminBoardGames/Index");
            }
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var publishers = _context.Publishers.ToList();
            ViewBag.Publishers = new SelectList(publishers, "PublisherId", "PublisherName");
            return View(boardGame);
        }

        // GET: Admin/AdminBoardGames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames.FindAsync(id);
            if (boardGame == null)
            {
                return NotFound();
            }
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var publishers = _context.Publishers.ToList();
            ViewBag.Publishers = new SelectList(publishers, "PublisherId", "PublisherName");
            return View(boardGame);
        }

        // POST: Admin/AdminBoardGames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BoardGame boardGame, IFormFile image1, IFormFile image2, IFormFile image3)
        {
            ModelState.Remove("image1");
            ModelState.Remove("image2");
            ModelState.Remove("image3");
            if (id != boardGame.BoardGameId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingboardGame = await _context.BoardGames.FindAsync(id); // 
                    //neu hinh anh ban null thi gan no bang hinh anh cu
                    if (image1 == null)
                    {
                        boardGame.Image1 = existingboardGame.Image1;
                    }
                    else
                    {
                        // Lưu hình ảnh mới
                        boardGame.Image1 = await SaveImage(image1);
                    }
                    //neu hinh anh ban null thi gan no bang hinh anh cu
                    if (image2 == null)
                    {
                        boardGame.Image2 = existingboardGame.Image2;
                    }
                    else
                    {
                        // Lưu hình ảnh mới
                        boardGame.Image2 = await SaveImage(image2);
                    }
                    //neu hinh anh ban null thi gan no bang hinh anh cu
                    if (image3 == null)
                    {
                        boardGame.Image3 = existingboardGame.Image3;
                    }
                    else
                    {
                        // Lưu hình ảnh mới
                        boardGame.Image3 = await SaveImage(image3);
                    }
                    existingboardGame.Name = boardGame.Name;
                    existingboardGame.Description = boardGame.Description;
                    existingboardGame.Image1 = boardGame.Image1;
                    existingboardGame.Image2 = boardGame.Image2;
                    existingboardGame.Image3 = boardGame.Image3;
                    existingboardGame.Price = boardGame.Price;
                    existingboardGame.DiscountPrice = boardGame.DiscountPrice;
                    existingboardGame.OriginPrice = boardGame.OriginPrice;
                    existingboardGame.Quantity = boardGame.Quantity;
                    existingboardGame.MinPlayers = boardGame.MinPlayers;
                    existingboardGame.MaxPlayers = boardGame.MaxPlayers;
                    existingboardGame.OrderIndex = boardGame.OrderIndex;
                    existingboardGame.Link = boardGame.Link;
                    existingboardGame.CategoryId = boardGame.CategoryId;
                    existingboardGame.PublisherId = boardGame.PublisherId;

                    
                    _context.BoardGames.Update(existingboardGame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardGameExists(boardGame.BoardGameId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminBoardGames/Index");
            }
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName",boardGame.CategoryId);
            var publishers = _context.Publishers.ToList();
            ViewBag.Publishers = new SelectList(publishers, "PublisherId", "PublisherName",boardGame.PublisherId);
            return View(boardGame);
        }

        // GET: Admin/AdminBoardGames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.BoardGameId == id);
            if (boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

        // POST: Admin/AdminBoardGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boardGame = await _context.BoardGames.FindAsync(id);
            if (boardGame != null)
            {
                _context.BoardGames.Remove(boardGame);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminBoardGames/Index");
        }

        private bool BoardGameExists(int id)
        {
            return _context.BoardGames.Any(e => e.BoardGameId == id);
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
