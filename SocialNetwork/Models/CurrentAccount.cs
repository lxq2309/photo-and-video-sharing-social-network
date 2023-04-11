namespace SocialNetwork.Models
{
    public static class CurrentAccount
    {
        public static Account account;
        private static SocialNetworkDbContext _context;
        public static void initSession(int accountId)
        {
            _context = new SocialNetworkDbContext();
            account = _context.Accounts.SingleOrDefault(x => x.AccountId == accountId);
        }

        public static void update()
        {
            _context.SaveChanges();
        }

        public static class Data
        {
            public static int getPostCount()
            {
                return new SocialNetworkDbContext().Posts.Count(x => x.AccountId == account.AccountId);
            }

            public static List<Notification> getListNotification()
            {
                return new SocialNetworkDbContext().Notifications.Where(x => x.AccountId == account.AccountId).OrderByDescending(x => x.NotiId).ToList();
            }

            public static List<ChatSession> getListChatSession()
            {
                update();
                return new SocialNetworkDbContext().Accounts
                    .Where(x => x.AccountId == account.AccountId)
                    .SelectMany(x => x.Chats)
                    .ToList();
            }

            public static Message getNewestMessage(ChatSession chatSession)
            {
                update();
                return new SocialNetworkDbContext().Messages.Where(x => x.ChatId == chatSession.ChatId).OrderByDescending(x => x.MessageId).FirstOrDefault();
            }
        }
    }
}
