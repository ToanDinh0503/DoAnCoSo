using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBG.Models;
using WebBG.ViewModels;
using System.Collections;

namespace WebBG.Controllers
{
    public class UserController : Controller
    {
        private readonly WebBgContext _context;
        public UserController(WebBgContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new UserViewModel
            {
                Menus = menus,
            };
            return View(viewModel);
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new UserViewModel
            {
                Menus = menus,
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            int userId = model.Register.UserId;
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = model.Register,
            };
            if (model.Register != null)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Register.Username);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại.";
                    return View(viewModel);
                }
                model.Register.Password = BCrypt.Net.BCrypt.HashPassword(model.Register.Password);
                model.Register.Permission = 0;
                model.Register.Hidden = 0;
                _context.Users.Add(model.Register);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "User");
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            if (model.Register != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Register.Username);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Register.Password, user.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Role, user.Permission.ToString()),
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                      return RedirectToAction("Index", "Home");        
                }
            }

            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = model.Register,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Info()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var users = new User();
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    users = await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
                }
            }
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = users,
            };
            return View(viewModel);
        }

        public IActionResult GetGeneralInfo()
        {
            var users = new User();
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    users = _context.Users.FirstOrDefault(m => m.FullName == username);
                }
            }
            var viewModel = new UserViewModel
            {
                Register = users,
            };
            return PartialView("_GetGeneralInfo", viewModel);
        }

        public IActionResult GetPurchaseHistory()
        {
            var users = new User();
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    users = _context.Users.FirstOrDefault(m => m.FullName == username);
                }
            }

            var orders = _context.Carts.Where(o => o.UserId == users.UserId).OrderByDescending(o => o.DateCreated).ToList();
            Hashtable statuses = new Hashtable();
            statuses.Add(1, "Chờ xác nhận");
            statuses.Add(2, "Đã xác nhận");
            statuses.Add(3, "Đang giao hàng");
            statuses.Add(4, "Đã nhận hàng");
            statuses.Add(0, "Đã huỷ");

            ViewBag.MyHashTable = statuses;
            var viewModel = new UserViewModel
            {
                Register = users,
                Carts = orders
            };
            return PartialView("_GetPurchaseHistory",viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var user = await GetUserFromContext();
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = user,
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserViewModel model)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var user = await GetUserFromContext();
            if (user == null)
            {
                return RedirectToAction("Logout", "User");
            }

            user.FullName = model.Register.FullName;
            user.Address = model.Register.Address;
            user.Email = model.Register.Email;
            user.Phone = model.Register.Phone;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Info", "User");
        }

        private async Task<User> GetUserFromContext()
        {
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    return await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
                }
            }
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var user = await GetUserFromContext();
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = user,
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserViewModel model)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var user = await GetUserFromContext();
            if (user == null)
            {
                return RedirectToAction("Logout", "User");
            }

            if (model.Register != null)
            {
                if (BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Info", "User");
                }
            }

            ViewBag.ErrorMessage = "Mật khẩu cũ không đúng.";
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = user,
            };
            return View(viewModel);
        }

    }

}
