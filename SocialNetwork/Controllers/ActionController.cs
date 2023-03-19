using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers
{
    public class ActionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search(string? searchText)
        {
            // ... thực hien tra ve ket qua tim kiem
            return View();
        }
    }
}
