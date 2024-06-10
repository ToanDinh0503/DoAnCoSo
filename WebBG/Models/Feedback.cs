using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBG.Models;

public partial class Feedback
{

    public int FeedbackId { get; set; }
    [Display(Name = "Người dùng")]
    public int UserId { get; set; }
    [Display(Name = "Boardgame")]
    public int BoardGameId { get; set; }
    [Display(Name = "Nội dung")]
    public string? Content { get; set; }
    [Display(Name = "Đánh giá")]
    public int? Rating { get; set; }
    [Display(Name = "Ngày đăng")]
    public DateTime? Datebegin { get; set; }
    [Display(Name = "Thứ tự")]
    public int? OrderIndex { get; set; }

    public virtual BoardGame BoardGame { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
