using SocialNetwork.Models;

namespace SocialNetwork.ViewModels
{
    public class ChatSessionMessagesViewModel
    {
        SocialNetworkDbContext _context = new SocialNetworkDbContext();
        public List<KeyValuePair <ChatSession, Account>> listChatSessitonAccount { get; set; }
        public List<Message> listMessage { get; set; }
        public Account partner { get;set; }
        public int chatID { get; set; }

        public ChatSessionMessagesViewModel(List<KeyValuePair<ChatSession, Account>> sessions, List<Message> messages, Account account)
        {
            listChatSessitonAccount = sessions;
            listMessage = messages;
            partner = account;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
