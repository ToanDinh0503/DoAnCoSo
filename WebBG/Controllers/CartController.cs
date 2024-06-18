using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBG.Models;
using WebBG.ViewModels;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection.Metadata;
using System.Net.WebSockets;
using WebBG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using NanoidDotNet;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Text.Json.Nodes;


namespace WebBG.Controllers
{
    public class CartController : Controller
    {
        private readonly WebBgContext _context;
        private readonly IVnPayService _vnPayservice;
        private const string CartSession = "CartSession";
      
        public CartController(WebBgContext context, IVnPayService vnpayService)
        {
            _context = context;
            _vnPayservice = vnpayService;
           
        }

        public IActionResult AddItem(int ProductId, int Quantity)
        {
            Quantity = 1;
            //tim san pham
            var product = _context.BoardGames.Find(ProductId);

            //lay ra gio hang trong sesion
            var cart = HttpContext.Session.GetString(CartSession);
            var list = new List<CartItem>();

            if (!string.IsNullOrEmpty(cart))
            {
                //giai gio hang ra
                list = JsonConvert.DeserializeObject<List<CartItem>>(cart);

                // kiem tra san phẩm có trong giỏ hàng chưa
                var cartItem = list.FirstOrDefault(x => x.Product.BoardGameId == ProductId);
                if (cartItem != null)
                {
                    // tính tổng số lượng nếu sản phẩm dược thêm vào
                    var totalQuantity = cartItem.Quantity + Quantity;

                    // kiểm tra coi tổng số lượng có vượt qua số lượng tồn của sp ko
                    if (totalQuantity > product.Quantity)
                    {
                        TempData["Error"] = "KHÔNG ĐỦ SẢN PHẨM.";
                        return RedirectToAction("Index");
                    }

                    // Cập nhật số lượng của sản phẩm trong session
                    cartItem.Quantity = totalQuantity;
                }
                else
                {
                    // kiểm tra coi tổng số lượng có vượt qua số lượng tồn của sp ko
                    if (Quantity > product.Quantity)
                    {
                        TempData["Error"] = "KHÔNG ĐỦ SẢN PHẨM.";
                        return RedirectToAction("Index");
                    }

                    // Thêm sản phẩm mới vào list
                    var item = new CartItem
                    {
                        Product = product,
                        Quantity = Quantity
                    };
                    list.Add(item);
                }
            }
            else
            {
                // kiểm tra coi tổng số lượng có vượt qua số lượng tồn của sp ko
                if (Quantity > product.Quantity)
                {
                    TempData["Error"] = "KHÔNG ĐỦ SẢN PHẨM.";
                    return RedirectToAction("Index");
                }

                // Thêm sản phẩm mới vào list
                var item = new CartItem
                {
                    Product = product,
                    Quantity = Quantity
                };
                list.Add(item);
            }

            // giải giỏ hàng vao session
            HttpContext.Session.SetString(CartSession, JsonConvert.SerializeObject(list));

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var cart = HttpContext.Session.GetString(CartSession);
            var list = new List<CartItem>();
            if (!string.IsNullOrEmpty(cart))
            {
                list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            }
            var cartViewModel = new CartViewModel
            {
                Menus = menus,
                Blogs = blogs,
                CartItems = list
            };
            return View(cartViewModel);
        }
        public IActionResult DeleteAll()
        {
            HttpContext.Session.Remove(CartSession);
            return Json(new
            {
                status = true
            });
        }
        public IActionResult Delete(int id)
        {
            var sessionCart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString(CartSession));
            /*   foreach (var cart in sessionCart.)
               {
                   post.Blog = null;
               }*/
            sessionCart.RemoveAll(x => x.Product.BoardGameId == id);
            HttpContext.Session.SetString(CartSession, JsonConvert.SerializeObject(sessionCart));
            return Json(new
            {
                status = true
            });
        }

        public IActionResult Update(string cartModel)
        {
            var jsonCart = JsonConvert.DeserializeObject<List<CartItem>>(cartModel);
            var sessionCart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString(CartSession));
            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.BoardGameId == item.Product.BoardGameId);
                var boardGame = _context.BoardGames.FirstOrDefault(x => x.BoardGameId == item.Product.BoardGameId);

                if (boardGame != null && boardGame.Quantity >= jsonItem.Quantity)
                {
                    if (jsonItem != null)
                    {
                        item.Quantity = jsonItem.Quantity;
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = false
                    });
                }
            }
            HttpContext.Session.SetString(CartSession, JsonConvert.SerializeObject(sessionCart));
            return Json(new
            {
                status = true
            });
        }


        [HttpGet]
        public async Task<IActionResult> Order(string name)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var cart = HttpContext.Session.GetString(CartSession);
            var list = new List<CartItem>();
            User user = new User();

            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                    user = await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
            }

            if (!string.IsNullOrEmpty(cart))
            {
                list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            }
            var cartViewModel = new CartViewModel
            {
                Menus = menus,
                Blogs = blogs,
                CartItems = list,
                User = user
            };
            return View(cartViewModel);
        }

        private string GenerateUniqueCartId()
        {
            string cartId;
            do
            {
                cartId = Nanoid.Generate(alphabet: "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", 15); // Tạo mã ID mới
            } while (_context.Carts.Any(c => c.CartId == cartId)); // Kiểm tra xem mã ID đã tồn tại trong cơ sở dữ liệu chưa

            return cartId;
        }


        [HttpPost]
        public async Task<IActionResult> TienHanhOrder(string PromoCode, Decimal Amount, Decimal DiscountedPrice, string CustomerName, string CustomerAddress, string CustomerEmail, string CustomerPhone, string paymentMethod = "COD")
        {
            var order = new Cart(); //Tạo mới đơn hàng

            order.CartId = GenerateUniqueCartId();
            order.Hidden = 0;
            order.DateCreated = DateTime.Now;
            order.PromoCode = PromoCode;
            order.CustomerName = CustomerName;
            order.CustomerEmail = CustomerEmail;
            order.CustomerPhone = CustomerPhone;
            order.CustomerAddress = CustomerAddress;
            order.SubTotal = Amount;
            order.DiscountedPrice = DiscountedPrice;
            order.isPaid = false;
            order.Status = 1; //Đang chờ xử lý

            var users = new User(); 

            if (User.Identity.IsAuthenticated) //xác thực user
            {
                string username = User.Identity.Name;
                if (username != null)
                    users = await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

            order.UserId = users.UserId;
            

            //kiem tra coi thanh toan kieu nao
            if (paymentMethod == "Thanh toán VNPay")/*Ngân hàng	NCB
                                                    Số thẻ	9704198526191432198
                                                    Tên chủ thẻ	NGUYEN VAN A
                                                    Ngày phát hành	07/15
                                                    Mật khẩu OTP	123456*/
            {
                order.PaymentMethod = "Thanh toán qua VNPay";
                order.isPaid = true;

                TempData["Order"] = JsonConvert.SerializeObject(order);

                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = Convert.ToDouble(Amount - DiscountedPrice),
                    CreatedDate = DateTime.Now,
                    Description = "Thanh toán đơn hàng",
                    FullName = users.FullName,
                    OrderId = _context.Payments.Count() + 1
                };
              

                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            else
            {
                try
                {
                    order.PaymentMethod = "Thanh toán khi nhận hàng";
                    order.isPaid = false;
                    _context.Carts.Add(order);
                    _context.SaveChanges();
                    var id = order.CartId;
                    var cart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString(CartSession));
                    foreach (var item in cart)
                    {
                        var detail = new CartDetail();
                        var boardGame = _context.BoardGames.FirstOrDefault(x => x.BoardGameId == item.Product.BoardGameId);

                        if (boardGame != null)
                        {
                            detail.BoardGameId = item.Product.BoardGameId;
                            detail.CartId = id;
                            detail.Quantity = item.Quantity;

                            // them detail vao
                            _context.CartDetails.Add(detail);

                            // Update the ProductQuantity
                            if (boardGame.Quantity >= item.Quantity)
                            {
                                boardGame.Quantity -= item.Quantity;
                                _context.BoardGames.Update(boardGame);
                            }
                            else
                            {
                                // xu lý không đủ sản phẩm
                                throw new InvalidOperationException("Không đủ sản phẩm");
                            }

                            // Save changes after each item is processed
                            _context.SaveChanges();
                        }

                    }

                   /* var promoCode = _context.Promotion.SingleOrDefault(p => p.PromoCode == PromoCode);
                    Payment payment = new Payment();
                    payment.CartId = id;
                    payment.Amount = order.SubTotal - order.DiscountedPrice;
                    payment.PromoId = promoCode.PromoId;
                    payment.UserId = order.UserId;
                    payment.PaymentDate = DateTime.Now;
                    _context.Payments.Add(payment);
                    _context.SaveChanges(true);*/


                }
                catch (Exception ex)
                {
                    throw;
                }


                return Redirect("/hoan-thanh/");
            }
        }

        public async Task<IActionResult> Success()
        {
            DeleteAll();
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var cart = HttpContext.Session.GetString(CartSession);
            var list = new List<CartItem>();
            if (!string.IsNullOrEmpty(cart))
            {
                list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            }
            var cartViewModel = new CartViewModel
            {
                Menus = menus,
                Blogs = blogs,
                CartItems = list,
            };

            return View(cartViewModel);
        }

        public async Task<IActionResult> Success_VnPay()
        {
            DeleteAll();
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var cart = HttpContext.Session.GetString(CartSession);
            var list = new List<CartItem>();
            if (!string.IsNullOrEmpty(cart))
            {
                list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            }
            var cartViewModel = new CartViewModel
            {
                Menus = menus,
                Blogs = blogs,
                CartItems = list,
            };

            return View(cartViewModel);
        }


        /*public decimal? CalToTalPrice(List<CartItem> cartItems, string proCode)
        {
            decimal? totalPrice = 0;

            foreach (var item in cartItems)
            {
                if (item.Product.DiscountPrice != null)
                {
                    totalPrice += item.Product.DiscountPrice * item.Quantity;
                }
                else
                {
                    totalPrice += item.Product.Price * item.Quantity;
}
                }

            return totalPrice;
        }*/
        public async Task<IActionResult> PaymentFail()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();

            var cartViewModel = new CartViewModel
            {
                Menus = menus,
                Blogs = blogs,

            };
            return View(cartViewModel);
        }

        [Authorize]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            //neu giao dich khong thanh cong
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay";
                return Redirect("~Cart/PaymentFail");
            }

            var order = new Cart();
            var payment = new Payment();
            var users = new User();
            var orderJson = TempData["Order"] as string;
   
            if (!string.IsNullOrEmpty(orderJson))
            {
                order = JsonConvert.DeserializeObject<Cart>(orderJson);

            }
     


            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                    users = await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

            order.UserId = users.UserId;
            try
            {
                _context.Carts.Add(order);
                _context.SaveChanges();
                var id = order.CartId;
                var cart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString(CartSession));
                foreach (var item in cart)
                {
                    var detail = new CartDetail();
                    var boardGame = _context.BoardGames.FirstOrDefault(x => x.BoardGameId == item.Product.BoardGameId);

                    if (boardGame != null)
                    {
                        detail.BoardGameId = item.Product.BoardGameId;
                        detail.CartId = id;
                        detail.Quantity = item.Quantity;

                        // them detail vao
                        _context.CartDetails.Add(detail);

                        // Update the ProductQuantity
                        if (boardGame.Quantity >= item.Quantity)
                        {
                            boardGame.Quantity -= item.Quantity;
                            _context.BoardGames.Update(boardGame);
                        }
                        else
                        {
                            // xu lý không đủ sản phẩm
                            throw new InvalidOperationException("Không đủ sản phẩm");
                        }

                        // Save changes after each item is processed
                        _context.SaveChanges();
                    }
                }
               
                
                payment.CartId = id;
                payment.Amount = order.SubTotal - order.DiscountedPrice;
                payment.UserId = order.UserId;
                payment.PaymentDate = DateTime.Now;
                _context.Payments.Add(payment);
                _context.SaveChanges(true);
                //payment.PromoId = promoCode != null ? promoCode.PromoId : null;

          
            }
            catch (Exception ex)
            {
                throw;
            }

            return Redirect("~/Cart/Success_VnPay");
        }

        public IActionResult GetCartItemCount()
        {
            var cart = HttpContext.Session.GetString("CartSession");
            int itemCount = 0;
            if (!string.IsNullOrEmpty(cart))
            {
                var list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
                itemCount = list.Sum(x => x.Quantity);
            }
            return Json(itemCount);
        }


        [HttpGet]
        public JsonResult ApplyPromoCode(string code, decimal? totalPrice)
        {
            if(String.IsNullOrEmpty(code))
            {
                return Json(new { status = false});
            }
            
            decimal? finalPrice = totalPrice ?? 0;
            decimal? discounted = 0;

            var promo = _context.Promotion.SingleOrDefault(p => p.PromoCode == code);

            if (promo != null)
            {
                if(promo.ExpirationDate < DateTime.Now)
                {
                    return Json(new { status = false, error = "Mã khuyến mãi đã hết hạn" });
                }

                if (promo.DiscountValue != null)
                {
                    discounted = promo.DiscountValue;
                    if (finalPrice != null)
                    {
                        finalPrice -= promo.DiscountValue;
                        if (finalPrice < 0)
                        {
                            finalPrice = 0;
                        }
                    }
                }
                else if (promo.DiscountPercent != null)
                {
                    discounted = promo.DiscountPercent * finalPrice;
                    if (finalPrice != null)
                    {
                        finalPrice -= finalPrice * promo.DiscountPercent;
                    }
                }
            }
            return Json(new { status = true, finalPrice = finalPrice, discounted = discounted });
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

        [HttpGet]
        public async Task<IActionResult> DisplayOrderDetailForCustomer(string id)
        {

            var orders = await _context.Carts.Include(cd => cd.CartDetails).ThenInclude(p => p.Product).ToListAsync();
            var orderToShow = orders.SingleOrDefault(o => o.CartId == id);
            if (orderToShow == null)
            {
                return NotFound();
            }


            return PartialView("_OrderDetails", orderToShow);
        }

        public async Task<IActionResult> CancelOrderConfirm(string id)
        {
            var order = await _context.Carts.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(string id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra";
                return Redirect("/thong-tin");
            }
            try
            {
                var orderExisted = await _context.Carts.FindAsync(id);
                if(!(orderExisted.Status == 1 || orderExisted.Status == 2)) {
                    ViewBag.ErrorMessage = "Không thể huỷ đơn hàng này";
                    return Redirect("/thong-tin");
                }
                orderExisted.Status = 0;
                _context.Carts.Update(orderExisted);
                await _context.SaveChangesAsync();
                ViewBag.SuccessMessage = "Đơn hàng đã được hủy thành công.";

                return Redirect("/thong-tin");
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
