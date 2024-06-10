using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBG.Models;

public partial class Category
{
    [Display(Name = "Mã")]
    public int CategoryId { get; set; }
    [Display(Name = "Tên loại")]
    public string? CategoryName { get; set; }
    [Display(Name = "Thứ tự")]
    public int? OrderIndex { get; set; }
    [Display(Name = "Liên kết")]
    public string? Link { get; set; }
    [Display(Name = "Ẩn/Hiện")]
    public int? Hidden { get; set; }

    public virtual ICollection<BoardGame> BoardGames { get; set; } = new List<BoardGame>();
}
