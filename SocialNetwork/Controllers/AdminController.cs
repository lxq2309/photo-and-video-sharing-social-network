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
            int pageNumber = page == null || page < 1 ? 1 : page.Value;
            int pageSise = 3;
            var listAccount = context.Accounts.AsNoTracking().OrderBy(x => x.AccountId).ToList();
            PagedList<Account> list = new PagedList<Account>(listAccount, pageNumber, pageSise);
            return View(list);
        }
    }
}
