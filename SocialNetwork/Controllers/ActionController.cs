using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Models.Authentication;
using SocialNetwork.ViewModels;

namespace SocialNetwork.Controllers
{
    public class ActionController : Controller
    {
        SocialNetworkDbContext db = new SocialNetworkDbContext();
        public IActionResult Index()
        {
            return View();
        }
        [Authentication]
        public IActionResult Search(string? searchText)
        {
            List<Account> lstAccount;
            List<Post> lstPost;

            // ... thực hien tra ve ket qua tim kiem
            if (searchText == null)
            {
                lstAccount = db.Accounts.ToList();
                lstPost = db.Posts.ToList();
                
            }
            else
            {
                lstAccount = db.Accounts.ToList().Where(x => x.FullName.Contains(searchText)).ToList();
                lstPost = db.Posts.ToList().Where(x => x.Content.Contains(searchText)).ToList();
            }

            var result = new SearchResultViewModel(lstAccount, lstPost);
            return View(result);    
        }
    }
}
