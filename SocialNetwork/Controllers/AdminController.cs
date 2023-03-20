using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;
using X.PagedList;

namespace SocialNetwork.Controllers
{
    public class AdminController : Controller
    {

        SocialNetworkDbContext context = new SocialNetworkDbContext();
        public IActionResult Index(int? page)
        {
            CurrentAccount.initSession(1);
            

            if(CurrentAccount.account.IsAdmin == true)
            {
                int pageNumber = page == null || page < 1 ? 1 : page.Value;
                int pageSise = 3;
                var listAccount = context.Accounts.AsNoTracking().OrderBy(x => x.AccountId).ToList();
                PagedList<Account> list = new PagedList<Account>(listAccount, pageNumber, pageSise);
                return View(list);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult BanAccount(int? accountId)
        {

            if (CurrentAccount.account.IsAdmin == true)
            {
                Account accountBan = context.Accounts.SingleOrDefault(x => x.AccountId == accountId);
                if (accountBan != null && accountBan.IsAdmin == false)
                {
                    accountBan.IsBanned = !accountBan.IsBanned;
                    context.Entry(accountBan).State = EntityState.Modified;
                    context.SaveChanges();
                    TempData["Message"] = "Changes has been saved ";
                    return RedirectToAction("Index");
                }
                TempData["Message"] = "Changes hasn't been saved !!!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
