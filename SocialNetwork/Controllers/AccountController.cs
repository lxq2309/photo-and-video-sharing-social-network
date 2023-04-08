using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;
using SocialNetwork.Models.Authentication;


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
                ModelState.AddModelError("Email", "Invalid email or password");
                return View();
            }
            return RedirectToAction("Index", "Home");
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
        [Authentication]
        public IActionResult Profile(int accountId)
        {
            int maxAccountId = db.Accounts.Max(x => x.AccountId);
            // Xử lí trường hợp accountId bị null hoặc < 1 hoặc > maxAccountId
            if (accountId == null || accountId < 1 || accountId > maxAccountId)
            {
                accountId = CurrentAccount.account.AccountId;
            }
            // chắc là sẽ thêm tham số mã tài khoản nhận vào ở đây
            var account = db.Accounts.SingleOrDefault(x => x.AccountId == accountId);

            int postCount = db.Posts.Count(x => x.AccountId == accountId);
            ViewBag.PostCount = postCount;

            // Lấy danh sách các post detail của tài khoản
            var lstPost = db.Posts.Where(x => x.AccountId == accountId).ToList();
            List<PostDetailViewModel> lstPostDetail = new List<PostDetailViewModel>();
            foreach (var item in lstPost)
            {
                lstPostDetail.Add(new PostDetailViewModel(item));
            }
            ViewBag.ListPostDetail = lstPostDetail;

            // Kiểm tra có đang theo dõi tài khoản này hay không
            bool following = db.Relationships.Where(x => x.SourceAccountId == CurrentAccount.account.AccountId
                                                      && x.TargetAccountId == accountId)
                                             .ToList()
                                             .Count != 0;
            ViewBag.Following = following;
            return View(account);
        }

        // =================== Setting ===================
        [Authentication]
        public IActionResult Setting()
        {
            var account = db.Accounts.SingleOrDefault(x => x.Email == CurrentAccount.account.Email);
            return View(account);
        }

        [HttpPost]
        [Authentication]
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
