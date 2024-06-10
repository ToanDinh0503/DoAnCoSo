using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBG.Models;

public partial class Payment
{
    [Display(Name = "Mã hóa đơn")]
    public int PaymentId { get; set; }

    [Display(Name = "Mã giỏ hàng")]
    public string? CartId { get; set; }
    public int? PromoId { get; set; }
    public int? UserId { get; set; }

    [Display(Name = "Số tiền")]
    public decimal Amount { get; set; }

    [Display(Name = "Mã giao dịch")]
    public string? TransactionId { get; set; }

    [Display(Name = "Ngày Thanh toán")]
    public DateTime PaymentDate { get; set; }

    public virtual Cart? Cart { get; set; } = null!;

    public virtual Promotion? Promotion { get; set; }

    public virtual User? User { get; set; }
}
