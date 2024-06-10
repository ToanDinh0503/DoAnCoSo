using WebBG.Models;
namespace WebBG.ViewModels
{
    public class BlogViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<Blog> Bl { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
