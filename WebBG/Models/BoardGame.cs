using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBG.Models;

public partial class BoardGame
{
    [Display(Name = "Mã boardgame")]
    public int BoardGameId { get; set; }

    [Display(Name = "Tên boardgame")]
    public string? Name { get; set; }

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Giá Gốc")]
    public decimal? OriginPrice { get; set; }

    [Display(Name = "Giá")]
    public decimal? Price { get; set; }

    [Display(Name = "Giá khuyến mãi")]
    public decimal? DiscountPrice { get; set; }



    [Display(Name = "Số lượng")]
    public int? Quantity { get; set; }

    [Display(Name = "Tối thiểu người chơi")]
    public int? MinPlayers { get; set; }
    [Display(Name = "Tối đa")]
    public int? MaxPlayers { get; set; }
    [Display(Name = "Tuổi")]
    public string? Age { get; set; }
    [Display(Name = "Hình ảnh")]
    public string? Image1 { get; set; }
    [Display(Name = "Hình ảnh")]
    public string? Image2 { get; set; }
    [Display(Name = "Hình ảnh")]
    public string? Image3 { get; set; }
    [Display(Name = "Thứ tự")]
    public int? OrderIndex { get; set; }

    [Display(Name = "Liên kết")]
    public string? Link { get; set; }
    [Display(Name = "Ẩn/Hiện")]
    public int? Hidden { get; set; }

    public int? CategoryId { get; set; }

    public int? PublisherId { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Publisher? Publisher { get; set; }

   
}
