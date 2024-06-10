using WebBG.Models;
namespace WebBG.ViewModels
{
    public class ProductViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<BoardGame> Bg { get; set; }
        public List<Feedback> Fb { get; set; }
        public String cateName { get; set; }

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int BoardGameId { get; set; }
    }
}
