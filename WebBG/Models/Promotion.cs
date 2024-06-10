using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBG.Models;

public partial class Promotion
{
    [Display(Name = "Mã khuyến mãi")]
    public int PromoId { get; set; }

    [Display(Name = "Code khuyến mãi")]
    [Required(ErrorMessage = "Code khuyến mãi không được để trống.")]
    [StringLength(10, ErrorMessage = "Mã khuyến mãi chỉ tối đa 10 ký tự.")]
    public string? PromoCode { get; set; }

    [Display(Name = "% OFF")]
    public decimal? DiscountPercent { get; set; }

    [Display(Name = "Giá trị giảm")]
    public decimal? DiscountValue { get; set; }

    [Display(Name = "Ngày hết hạn")]
    public DateTime ExpirationDate { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

}

