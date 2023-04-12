using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
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
                lstPost = db.Posts.Where(x => x.IsDeleted == false).ToList();
            }
            else
            {
                lstAccount = db.Accounts.Where(x => x.FullName.Contains(searchText)).ToList();
                // chỗ này đang bug
                lstPost = db.Posts
                            .FromSqlRaw($"SELECT * FROM Post WHERE IsDeleted = 0 AND Content IS NOT NULL AND CAST(Content AS NVARCHAR(MAX)) LIKE N'%{searchText}%'")
                            .ToList();
            }

            var lstPostDetail = new List<PostDetailViewModel>();
            foreach (var item in lstPost)
            {
                lstPostDetail.Add(new PostDetailViewModel(item));
            }
            var result = new SearchResultViewModel(lstAccount, lstPostDetail);
            return View(result);
        }
    }
}
