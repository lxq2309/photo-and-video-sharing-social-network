using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;

namespace SocialNetwork.ViewComponents
{
    public class PostDetailViewComponent : ViewComponent
    {
        SocialNetworkDbContext _context = new SocialNetworkDbContext();
        public PostDetailViewComponent() { }

        public IViewComponentResult Invoke()
        {
            int currentAccountID = CurrentAccount.account.AccountId;
            var lstAccountIDFollow = _context.Relationships.Where(x=>x.SourceAccountId == currentAccountID && x.TypeId == 2).Select(x=>x.TargetAccountId).ToList();
            var lstPost = _context.Posts.Where(x => (lstAccountIDFollow.Contains(x.AccountId) || x.AccountId == currentAccountID) && x.IsDeleted == false).OrderByDescending(x => x.CreateAt);
            var lstPostDetail = new List<PostDetailViewModel>();
            foreach (var post in lstPost)
            {
                lstPostDetail.Add(new PostDetailViewModel(post));
            }
            return View(lstPostDetail);
        }
    }
}
