using System.Collections.Generic;
using WebBG.Models;

namespace WebBG.ViewModels
{
    public class HomeViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<Slider> Sliders { get; set; }
        public Dictionary<Category, List<BoardGame>> CategoryProducts { get; set; }
    }
}
