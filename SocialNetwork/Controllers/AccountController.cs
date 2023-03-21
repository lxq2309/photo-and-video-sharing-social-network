using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        SocialNetworkDbContext db = new SocialNetworkDbContext();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("", "");
            }
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
