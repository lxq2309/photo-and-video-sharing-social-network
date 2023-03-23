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
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string email, string password)
        {
            var account = db.Accounts.SingleOrDefault(x => x.Email == email && x.Password == password);
            if (account == null)
            {
                ModelState.AddModelError("Email", "Invalid email or password");
                return View();
            }

            CurrentAccount.initSession(account.AccountId);

            return RedirectToAction("Index", "Home");
        }

        // =================== Logout ===================
        public IActionResult Logout()
        {
            // xử lý action logout sau đó chuyển về view login 
            return RedirectToAction("Login", "Account");
        }

        // =================== Register ===================
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Account account)
        {
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
        public IActionResult Profile()
        {
            // chắc là sẽ thêm tham số mã tài khoản nhận vào ở đây

            return View();
        }

        // =================== Setting ===================
        public IActionResult Setting()
        {
            var account = db.Accounts.SingleOrDefault(x => x.Email == CurrentAccount.account.Email);

            return View(account);
        }

        //[HttpPost]
        //public IActionResult setting(Account account)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(account).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    return View();
        //}

    }
}
