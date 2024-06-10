using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebBG.Models
{
    public partial class User
    {
        [Display(Name = "Mã người dùng")]
        public int UserId { get; set; }
        [Display(Name = "Tên người dùng")]
        public string? Username { get; set; }
        [Display(Name = "Mật Khẩu")]
        public string? Password { get; set; }
        [Display(Name = "Tên đầy đủ")]
        public string? FullName { get; set; }
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        public string? Email { get; set; }
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Range(0, 1, ErrorMessage = "Permission must be either 0 or 1")]
        [Display(Name = "Quyền")]
        public int? Permission { get; set; }
        [Display(Name = "Ẩn/hiện")]
        public int? Hidden { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

