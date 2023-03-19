namespace SocialNetwork.Models
{
    public class CurrentAccount
    {
        public static Account account;

        public static void initSession(int accountId)
        {
            SocialNetworkDbContext context = new SocialNetworkDbContext();
            account = context.Accounts.SingleOrDefault(x => x.AccountId == accountId);
        }
        
    }
}
