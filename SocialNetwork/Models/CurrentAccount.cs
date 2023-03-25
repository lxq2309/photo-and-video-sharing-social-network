namespace SocialNetwork.Models
{
    public static class CurrentAccount
    {
        public static Account account;
        private static SocialNetworkDbContext _context = new SocialNetworkDbContext();
        public static void initSession(int accountId)
        {
            account = _context.Accounts.SingleOrDefault(x => x.AccountId == accountId);
        }

        public static class Data
        {
            public static int getPostCount()
            {
                return _context.Posts.Count(x => x.AccountId == account.AccountId);
            }

            public static List<Notification> getListNotification()
            {
                return _context.Notifications.Where(x => x.AccountId == account.AccountId).ToList();
            }

            public static List<ChatSession> getListChatSession()
            {
                return _context.Accounts
                    .Where(x => x.AccountId == account.AccountId)
                    .SelectMany(x => x.Chats)
                    .ToList();
            }

            public static Message getNewestMessage(ChatSession chatSession)
            {
                return _context.Messages.Where(x => x.ChatId == chatSession.ChatId).OrderByDescending(x => x.MessageId).First();
            }
        }
    }
}
