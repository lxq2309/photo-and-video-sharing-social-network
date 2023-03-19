using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            // xử lý action logout sau đó chuyển về view login 
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Profile()
        {
            // chắc là sẽ thêm tham số mã tài khoản nhận vào ở đây

            return View();
        }
        public IActionResult Setting()
        {

            return View();
        }

    }
}
