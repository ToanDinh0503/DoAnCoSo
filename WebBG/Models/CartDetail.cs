using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBG.Models;

public partial class CartDetail
{
    [Display(Name = "Mã CT giỏ hàng")]
    public int CartDetailId { get; set; }
    [Display(Name = "Mã giỏ hàng")]
    public string CartId { get; set; }
    [Display(Name = "Boardgame")]
    public int BoardGameId { get; set; }
    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public decimal? Price { get; set; }

    //public virtual BoardGame BoardGame { get; set; } = null!;
    public BoardGame Product { get; set; }
}
