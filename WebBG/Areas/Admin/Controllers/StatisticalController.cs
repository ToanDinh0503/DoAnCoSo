using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using WebBG.Models;

namespace WebBG.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class StatisticalController : Controller
    {
        private WebBgContext db = new WebBgContext();
        public StatisticalController(WebBgContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            //ket bang lay ra cac thong tin can thiet
            var query = from p in db.Payments
                        join c in db.Carts
                        on p.CartId equals c.CartId
                        join cd in db.CartDetails
                        on c.CartId equals cd.CartId
                        join b in db.BoardGames
                        on cd.BoardGameId equals b.BoardGameId
                        select new {
                            //lay ra ngay thanh toan,so luong,gia,gia goc sp
                            PaymentDate = p.PaymentDate,
                            Quantity = cd.Quantity,
                            Price = b.Price,
                            OriginPrice = b.OriginPrice,
                            TotalPrice = p.Amount,
                        };
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime parsedFromDate))
            {
                //lay ra ngay bat dau và lọc ra
                query = query.Where(x => x.PaymentDate >= parsedFromDate);
            }

            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime parsedToDate))
            {
                //lay ra ngay ket thuc và lọc ra
                parsedToDate = parsedToDate.AddDays(1); // add 1 day to include the whole toDate
                query = query.Where(x => x.PaymentDate < parsedToDate);
            }

            var result = query.GroupBy(x => x.PaymentDate.Date).Select(x => new
            {
                Date = x.Key,
                //tong so tien von
                TotalBuy = x.Sum(y => y.Quantity * y.OriginPrice),
                //tong so tien ban
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date.Date,
                //doanh thu lay tu totalsell
                DoanhThu = x.TotalSell,
                //tinh ra loi nhuan
                LoiNhuan = x.TotalSell - x.TotalBuy,
            });


            return Json(new {Data = result});
        }
    }
}
