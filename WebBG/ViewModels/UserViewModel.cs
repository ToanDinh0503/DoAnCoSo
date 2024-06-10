using WebBG.Models;

namespace WebBG.ViewModels
{
    public class UserViewModel
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();
        public User Register { get; set; }
        public string OldPassword { get; set; } // Add this property
        public string NewPassword { get; set; } // Add this property

        public UserViewModel()
        {
            Register = new User();
        }

        public List<Cart> Carts { get; set; } = new List<Cart>();
    }
}
