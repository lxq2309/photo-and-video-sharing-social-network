using SocialNetwork.Models;

namespace SocialNetwork.ViewModels
{
    public class SearchResultViewModel
    {
        public List<Account> lstAccount { get; set; }
        public List<PostDetailViewModel> lstPost { get; set; }

        public SearchResultViewModel(List<Account> lstAccount, List<PostDetailViewModel> lstPost)
        {
            this.lstAccount = lstAccount;
            this.lstPost = lstPost;
        }
    }
}
