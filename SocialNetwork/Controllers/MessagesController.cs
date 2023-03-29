using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;
using System;
using System.Linq;

namespace SocialNetwork.Controllers
{
    public class MessagesController : Controller
    {
        SocialNetworkDbContext dbContext = new SocialNetworkDbContext();
        private readonly ILogger<MessagesController> _logger;
        public MessagesController(ILogger<MessagesController> logger)
        {
            _logger = logger;
        }
        
        [Route("Messages")]
        public IActionResult Index()
        {
            // Nhan vao id cuoc tro chuyen, neu co thi luu id vao TempData roi chuyen sang trang ChatSession
            // de hien thi cuoc hoi thoai cu the (lam cach nay de khong hien duong dan cu the tren thanh)
            // ...
            return RedirectToAction("ChatSession", "Messages");
        }

        [HttpGet]
        [Route("Messages/ChatSession")]
        public IActionResult ChatSession()
        {
            // Kiem tra tempdata xem co luu id cua chat session khong, neu co thi render ra lich su tin nhan
            // Nhan vao id cuoc tro chuyen, chuyen toi trang message cua cuoc tro chuyen do
            return RedirectToAction("ChatSession", new {chatId = -1});
        }

        [HttpGet]
        [Route("Messages/ChatSession/{chatId:int}")]
        public IActionResult ChatSession(int chatId)
        {
            List<ChatSession> chatSessions = CurrentAccount.Data.getListChatSession();
            List<KeyValuePair<ChatSession, Account>> chatSessionsAccount = new List<KeyValuePair<ChatSession, Account>>();
            Account partner = GetChatPartner(chatId);
            if (chatId > 0 && partner != null)
            {
                chatSessionsAccount.Add(new KeyValuePair<ChatSession, Account>(dbContext.ChatSessions.SingleOrDefault(x => x.ChatId == chatId)
                    , partner));
            }
            foreach (ChatSession item in chatSessions)
            {
                if (item.ChatId != chatId)
                {
                    chatSessionsAccount.Add(new KeyValuePair<ChatSession, Account>(item, GetChatPartner(item.ChatId)));
                }
            }
            
            ChatSessionMessagesViewModel chatSessionMessagesViewModel = new ChatSessionMessagesViewModel(
                chatSessionsAccount,
                dbContext.Messages.Where(x => x.ChatId == chatId).ToList(),
                partner
                );
            chatSessionMessagesViewModel.chatID = chatId;
            return View(chatSessionMessagesViewModel);
        }

        [HttpGet]
        [Route("Messages/account/{accountId:int}")]
        public IActionResult ChatSessionAccount(int accountId)
        {
            //lấy ra tài khoản đích đang muốn nhắn tin
            List<ChatSession> chatSessionsPartner = dbContext.Accounts.Where(x => x.AccountId == accountId).SelectMany(y => y.Chats).ToList();
            Account partner = dbContext.Accounts.SingleOrDefault(x => x.AccountId == accountId);
            foreach (var item in chatSessionsPartner)
            {
                foreach (var curr in CurrentAccount.Data.getListChatSession())
                    if (item.ChatId == curr.ChatId)
                    {
                        return RedirectToAction("ChatSession", new { chatId = item.ChatId}); ;
                    }
            }
            CreateNewChatSession(CurrentAccount.account, partner);

            return RedirectToAction("ChatSessionAccount", new { accountId = accountId });
        }

        public void CreateNewChatSession(Account currAcc, Account partner)
        {
            ChatSession tmp = new ChatSession();
            tmp.Name = currAcc.FullName + ", " + partner.FullName;
            
            dbContext.ChatSessions.Add(tmp);
            dbContext.SaveChanges();
            int newChatId = dbContext.ChatSessions.Max(x => x.ChatId);
            //tmp = dbContext.ChatSessions.SingleOrDefault(x => x.ChatId == newChatId);
            
            //tmp.Accounts.Add(partner);
            tmp.Accounts.Add(currAcc);
            tmp.Accounts.Add(partner);
            
            currAcc.Chats.Add(tmp);
            partner.Chats.Add(tmp); 
            dbContext.SaveChanges();
        }

        [HttpPost]
        public IActionResult SendMessage(string mess, int chatID)
        {
            Message message = new Message();
            message.ChatId = chatID;
            message.MessageContent = mess;
            message.CreateAt = DateTime.Now;
            message.AccountId = chatID;

            dbContext.Messages.Add(message);
            dbContext.ChatSessions.SingleOrDefault(x => x.ChatId == chatID).Messages.Add(message);
            //CurrentAccount.account.Messages.Add(message);
            dbContext.Accounts.SingleOrDefault(x => x.AccountId == CurrentAccount.account.AccountId).Messages.Add(message);
            
            dbContext.SaveChanges();
            return RedirectToAction("ChatSession", new {chatID = chatID });
        }

        [HttpGet]
        [Route("Messages/DeleteChatSession/{chatID}")]
        public IActionResult DeleteChatSession(int chatID)
        {
            dbContext.Messages.RemoveRange(dbContext.Messages.Where(x => x.ChatId == chatID));
            dbContext.SaveChanges();
            return RedirectToAction("ChatSession");
        }

        public Account GetChatPartner(int chatID)
        {   
            List<Account> accounts = dbContext.ChatSessions.Where(x => x.ChatId == chatID).SelectMany(x=>x.Accounts).ToList();
            foreach (Account item in accounts)
            {
                if (item.AccountId != CurrentAccount.account.AccountId)
                {
                    return item;
                }
            }
            return null;
        }

        [HttpGet]
        [Route("Messages/Search/{nameSearch}")]
        public IActionResult ChatSession(string nameSearch)
        {
            List<KeyValuePair<ChatSession, Account>> listSearch = new List<KeyValuePair<ChatSession, Account>>();
            foreach (var curr in CurrentAccount.Data.getListChatSession())
            {
                Account partner = GetChatPartner(curr.ChatId);
                if (partner!= null)
                {
                    if (curr.Name.Contains(nameSearch) || partner.DisplayName.Contains(nameSearch) || partner.FullName.Contains(nameSearch))
                    {
                        listSearch.Add(new KeyValuePair<ChatSession, Account>(curr, partner));
                    }
                }
            }
    
            return View(new ChatSessionMessagesViewModel(listSearch, null, null));
        }
    }
}
