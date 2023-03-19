using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.ViewComponents
{
    public class PostDetailViewComponent : ViewComponent
    {
        public PostDetailViewComponent() { }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
