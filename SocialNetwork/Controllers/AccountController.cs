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

        // =================== Login ===================
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("accountId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (HttpContext.Session.GetInt32("accountId") == null)
            {
                var account = db.Accounts.SingleOrDefault(x => x.Email == email && x.Password == password);
                if (account != null)
                {
                    HttpContext.Session.SetInt32("accountId", account.AccountId);
                    CurrentAccount.initSession(account.AccountId);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        // =================== Logout ===================
        public IActionResult Logout()
        {
            // xử lý action logout sau đó chuyển về view login 
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("accountId");
            return RedirectToAction("Login", "Account");
        }

        // =================== Register ===================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Account account)
        {
            // ModelState.AddModelError("Email", "");
            if (db.Accounts.FirstOrDefault(x => x.Email == account.Email) != null)
            {
                ModelState.AddModelError("Email", "Email has already been taken.");
                return View(account);
            }
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("", "");
            }
            return View();
        }

        // =================== Profile ===================
        public IActionResult Profile(int accountId)
        {
            // chắc là sẽ thêm tham số mã tài khoản nhận vào ở đây
            var account = db.Accounts.SingleOrDefault(x => x.AccountId == accountId);

            int postCount = db.Posts.Count(x => x.AccountId == accountId);
            ViewBag.PostCount = postCount;
            return View(account);
        }

        // =================== Setting ===================
        public IActionResult Setting()
        {
            var account = db.Accounts.SingleOrDefault(x => x.Email == CurrentAccount.account.Email);
            return View(account);
        }

        [HttpPost]
        public IActionResult setting(Account model)
        {
            var account = db.Accounts.SingleOrDefault(x => x.Email == CurrentAccount.account.Email);
            account.FullName = model.FullName;
            account.AboutMe = model.AboutMe;
            account.Location = model.Location;
            account.Phone = model.Phone;
            db.SaveChanges();
            return View(account);
        }

    }
}
