using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBG.Models;

public partial class Blog
{
    [Display(Name = "Mã blog")]
    public int BlogId { get; set; }

    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }
    [Display(Name = "Nội dung")]
    public string? Content { get; set; }

    [Display(Name = "Hình ảnh")]
    public string? Image { get; set; }
    [Display(Name = "Ngày đăng")]
    public DateTime? Datebegin { get; set; }

    [Display(Name = "Người dùng")]
    public int UserId { get; set; }
    [Display(Name = "Liên kết")]
    public string? Link { get; set; }
    [Display(Name = "Thứ tự")]
    public int? OrderIndex { get; set; }
    [Display(Name = "Ẩn/Hiện")]
    public int? Hidden { get; set; }

    public virtual User? User { get; set; } = null!;
}
