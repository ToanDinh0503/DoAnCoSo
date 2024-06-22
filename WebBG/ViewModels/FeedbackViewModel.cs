using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebBG.Models;
namespace WebBG.ViewModels
{
    public class FeedbackViewModel
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();
        public List<BoardGame> Bg { get; set; }
        public int BoardGameId { get; set; }


        public Feedback Fb { get; set; }

        public FeedbackViewModel()
        {
            Fb = new Feedback();
        }
    }
}
