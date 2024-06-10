using WebBG.Models;

namespace WebBG.ViewModels
{
    public class CartViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<CartItem> CartItems { get; set; }

        public Payment? Payment { get; set; }

        public User? User { get; set; }

        public Cart? Cart { get; set; }

        
    }
}
