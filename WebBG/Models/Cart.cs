using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace WebBG.Models;

public partial class Cart
{
    [Display(Name = "Mã đơn hàng")]
    [StringLength(15)]
    public string CartId { get; set; }
   
    [Display(Name = "Người dùng")]
    public int? UserId { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime DateCreated { get; set; }
    [Display(Name = "Ẩn/Hiện")]
    public int? Hidden { get; set; }


    [Display(Name = "Mã khuyến mãi")]
    public string? PromoCode { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? DiscountedPrice { get; set; } = 0;

    [Display(Name = "Trạng thái")]
    public int? Status { get; set; }


    [Display(Name = "Phương thưc thanh toán")]
    [StringLength(30)]
    public string? PaymentMethod { get; set; }
    public bool? isPaid { get; set; }

    [Display(Name = "Khách hàng")]
    public string? CustomerName { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? CustomerAddress { get; set; }

    [Display(Name = "Email")]
    public string? CustomerEmail { get; set; }

    [Display(Name = "Điện thoại")]
    public string? CustomerPhone { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
