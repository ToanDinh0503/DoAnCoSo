using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace WebBG.Models;

public partial class Publisher
{
    [Display(Name = "Mã NSX")]
    public int PublisherId { get; set; }

    [Display(Name = "Tên")]
    public string? PublisherName { get; set; }
    [Display(Name = "Thứ tự")]
    public int? OrderIndex { get; set; }
    [Display(Name = "Liên kết")]
    public string? Link { get; set; }
    [Display(Name = "Ẩn/Hiện")]
    public int? Hidden { get; set; }

    public virtual ICollection<BoardGame> BoardGames { get; set; } = new List<BoardGame>();
}
