using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
