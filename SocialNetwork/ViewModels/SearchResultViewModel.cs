using SocialNetwork.Models;

namespace SocialNetwork.ViewModels
{
    public class SearchResultViewModel
    {
        public List<Account> lstAccount { get; set; }
        public List<Post> lstPost { get; set; }

        public SearchResultViewModel(List<Account> lstAccount, List<Post> lstPost)
        {
            this.lstAccount = lstAccount;
            this.lstPost = lstPost;
        }
    }
}
