using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SocialNetwork.Models;
using SocialNetwork.Models.Authentication;
using System.Text;

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionAPIController : ControllerBase
    {
        SocialNetworkDbContext db = new SocialNetworkDbContext();

        [HttpGet]
        public IEnumerable<Post> GetAllPosts()
        {
            var posts = db.Posts.ToList();
            return posts;
        }

        // :) vailoz loi o cho nay 
        // nếu sử dụng cái {postid} trên này thì đường dẫn phải là https://localhost:7150/api/ActionAPI/1, tức là /postId ấy loz má
        //[HttpPost("{postId}")]


        // Còn viết k có tham số như này thì sẽ truyền dạng https://localhost:7150/api/ActionAPI?PostId=1, tức là ?postId = ấy
        [HttpPost]
        public Boolean LikePost(string postId)
        {
            var post = db.Posts.Include(x => x.Accounts).FirstOrDefault(x => x.PostId == int.Parse(postId));

            if (post != null)
            {
                foreach (var item in post.Accounts)
                {
                    if (item.AccountId.Equals(CurrentAccount.account.AccountId))
                    {
                        post.Accounts.Remove(item);
                        post.LikeCount = post.LikeCount - 1;
                        db.SaveChanges();
                        return false;
                    }
                }
                var account = db.Accounts.Include(x => x.Posts).FirstOrDefault(x => x.AccountId.Equals(CurrentAccount.account.AccountId));
                account.Posts.Add(post);
                post.LikeCount = post.LikeCount + 1;

                // thêm dữ liệu vào bảng Notification
                if (CurrentAccount.account.AccountId != post.AccountId)
                {
                    Notification newNoti = new Notification();
                    newNoti.PostId = post.PostId;
                    newNoti.Content = $"{CurrentAccount.account.FullName} đã thích bài viết của bạn";
                    newNoti.TypeNotification = 1;
                    newNoti.AccountId = post.AccountId;
                    db.Notifications.Add(newNoti);
                }
				db.SaveChanges();
                return true;
            }
            return false;
        }

        [Authentication]
        [HttpPost("addComment")]
        public IActionResult commentPost(Comment comment)
        {
            comment.AccountId = CurrentAccount.account.AccountId;
            if (ModelState.IsValid)
            {
                comment.CreateAt = DateTime.Now;
                db.Comments.Add(comment);
                var post = db.Posts.SingleOrDefault(x => x.PostId == comment.PostId);
                post.CommentCount = post.CommentCount + 1;

                // thêm dữ liệu vào bảng Notification
                if (CurrentAccount.account.AccountId != post.AccountId)
                {
                    Notification newNoti = new Notification();
                    newNoti.PostId = post.PostId;
                    newNoti.Content = $"{CurrentAccount.account.FullName} đã bình luận về bài viết của bạn";
                    newNoti.TypeNotification = 1;
                    newNoti.AccountId = post.AccountId;
                    db.Notifications.Add(newNoti);
                }
				db.SaveChanges();

                var account = db.Accounts.SingleOrDefault(x => x.AccountId == comment.AccountId);

                var data = new
                {
                    content = comment.Content,
                    accountImg = account.Avatar,
                    accountName = account.FullName
                };

                return new JsonResult(data);
            }
            return new JsonResult(null);
        }

        [Authentication]
        [HttpGet("Comment")]
        public IEnumerable<Comment> GetComment(string postID)
        {
            var lstComment = db.Comments.Where(x => x.PostId == int.Parse(postID)).ToList();

            return lstComment;
        }

        /*

                                   _
                        _ooOoo_
                       o8888888o
                       88" . "88
                       (| -_- |)
                       O\  =  /O
                    ____/`---'\____
                  .'  \\|     |//  `.
                 /  \\|||  :  |||//  \
                /  _||||| -:- |||||_  \
                |   | \\\  -  /'| |   |
                | \_|  `\`---'//  |_/ |
                \  .-\__ `-. -'__/-.  /
              ___`. .'  /--.--\  `. .'___
           ."" '<  `.___\_<|>_/___.' _> \"".
          | | :  `- \`. ;`. _/; .'/ /  .' ; |
          \  \ `-.   \_\_`. _.'_/_/  -' _.' /
===========`-.`___`-.__\ \___  /__.-'_.'_.-'================
                        `=--=-'
                     CODE KHÔNG BUG
         */

        //unfollow

        [Route("unfollow")]
        [HttpDelete]
        public bool Unfollow(int source, int target)
        {
            // Kiểm tra xem đã follow người này chưa
            var checkExist = db.Relationships.AsNoTracking().SingleOrDefault(x => x.SourceAccountId == source && x.TargetAccountId == target && x.TypeId == 2);
            if (checkExist == null)
            {
                // Nếu chưa follow thì không unfollow được
                return false;
            }
            // Nếu đã follow thì tiến hành unfollow
            string query = $"DELETE FROM Relationship " +
                           $"WHERE SourceAccountId = {source} AND TargetAccountId = {target}";
            db.Database.ExecuteSqlRaw(query);
            var targetAccount = db.Accounts.SingleOrDefault(x => x.AccountId == target);
            CurrentAccount.account.Following = CurrentAccount.account.Following - 1;
            CurrentAccount.update();
            targetAccount.Follower = targetAccount.Follower - 1;
            db.SaveChanges();
            return true;
        }

        //follow
        [Route("follow")]
        [HttpPost]
        public bool Follow(int source, int target)
        {
            // Kiểm tra xem đã follow người này chưa
            var checkExist = db.Relationships.AsNoTracking().SingleOrDefault(x => x.SourceAccountId == source && x.TargetAccountId == target && x.TypeId == 2);
            if (checkExist != null)
            {
                // Nếu đã follow rồi thì không cho follow nữa
                return false;
            }
            // Nếu chưa follow thì tiến hành follow
            string query = $"INSERT INTO Relationship(SourceAccountID,TargetAccountID,TypeID)" +
                           $" VALUES ({source}, {target}, 2)";
            db.Database.ExecuteSqlRaw(query);
            var targetAccount = db.Accounts.SingleOrDefault(x => x.AccountId == target);
            CurrentAccount.account.Following = CurrentAccount.account.Following + 1;
            CurrentAccount.update();
            targetAccount.Follower = targetAccount.Follower + 1;

			// thêm dữ liệu vào bảng Notification
			Notification newNoti = new Notification();
            newNoti.PostId = source;
            newNoti.Content = $"{CurrentAccount.account.FullName} đã bắt đầu theo dõi bạn";
            newNoti.TypeNotification = 2;
			newNoti.AccountId = target;
			db.Notifications.Add(newNoti);

			db.SaveChanges();
            return true;
        }

        //Block
        [Route("block")]
        [HttpPut]
        public bool Block(int source, int target)
        {
            string query;

            // Kiểm tra xem đã follow người này chưa
            var checkExist = db.Relationships.AsNoTracking().SingleOrDefault(x => x.SourceAccountId == source && x.TargetAccountId == target && x.TypeId == 2);
            if (checkExist == null)
            {
                // Nếu chưa follow thì thêm mới bản ghi
                query = $"INSERT INTO Relationship(SourceAccountID,TargetAccountID,TypeID)" +
                        $" VALUES ({source}, {target}, 3)";
            }
            else
            {
                // Nếu follow rồi thì điều chỉnh lại following và follower
                query = $"UPDATE Relationship " +
                        $"SET TypeID = 3 " +
                        $"WHERE SourceAccountId = {source} AND TargetAccountId = {target}";
                var targetAccount = db.Accounts.SingleOrDefault(x => x.AccountId == target);
                CurrentAccount.account.Following = CurrentAccount.account.Following - 1;
                CurrentAccount.update();
                targetAccount.Follower = targetAccount.Follower - 1;
            }
            db.Database.ExecuteSqlRaw(query);
            db.SaveChanges();
            return true;
        }

        //Unblock
        [Route("unblock")]
        [HttpDelete]
        public bool Unblock(int source, int target)
        {
            // Kiểm tra xem đã block người này chưa
            var checkExist = db.Relationships.AsNoTracking().SingleOrDefault(x => x.SourceAccountId == source && x.TargetAccountId == target && x.TypeId == 3);
            if (checkExist == null)
            {
                // Nếu chưa block thì không làm gì cả
                return false;
            }

            // Nếu đã block thì tiến hành unblock cho họ
            string query = $"DELETE FROM Relationship " +
                           $"WHERE SourceAccountId = {source} AND TargetAccountId = {target}";
            db.Database.ExecuteSqlRaw(query);
            db.SaveChanges();
            return true;
        }

        // Gửi yêu cầu follow
        [Route("request_follow")]
        [HttpPost]
        public bool RequestFollow(int source, int target) 
        {
            // Kiểm tra xem đã gửi yêu cầu follow trước đó hay chưa
            var checkExist = db.Relationships.AsNoTracking().SingleOrDefault(x => x.SourceAccountId == source && x.TargetAccountId == target && x.TypeId == 1);
            if (checkExist != null)
            {
                // Nếu đã gửi yêu cầu follow rồi thì không làm gì cả
                return false;
            }

            // Nếu chưa gửi yêu cầu follow thì tiến hành gửi
            string query = $"INSERT INTO Relationship(SourceAccountID,TargetAccountID,TypeID)" +
                           $" VALUES ({source}, {target}, 1)";
            db.Database.ExecuteSqlRaw(query);

			// thêm dữ liệu vào bảng Notification
			Notification newNoti = new Notification();
			newNoti.Content = $"{CurrentAccount.account.FullName} đã gửi yêu cầu theo dõi bạn";
			newNoti.TypeNotification = 3;
			newNoti.AccountId = target;
			db.Notifications.Add(newNoti);

			db.SaveChanges();
            return true;
        }

        // Huỷ yêu cầu
        [Route("cancel_request_follow")]
        [HttpDelete]
        public bool CancelRequestFollow(int source, int target)
        {
            // Kiểm tra xem đã gửi yêu cầu follow người này chưa
            var checkExist = db.Relationships.AsNoTracking().SingleOrDefault(x => x.SourceAccountId == source && x.TargetAccountId == target && x.TypeId == 1);
            if (checkExist == null)
            {
                // Nếu chưa gửi yêu cầu thì không làm gì cả
                return false;
            }

            // Nếu đã yêu cầu thì tiến hành huỷ yêu cầu
            string query = $"DELETE FROM Relationship " +
                           $"WHERE SourceAccountId = {source} AND TargetAccountId = {target}";
            db.Database.ExecuteSqlRaw(query);
            db.SaveChanges();
            return true;
        }

        // Chấp nhận yêu cầu
        [Route("accept_request_follow")]
        [HttpPut]
        public bool AcceptRequestFollow(int source, int target)
        {
            // Kiểm tra xem có yêu cầu follow không
            var checkExist = db.Relationships.AsNoTracking().SingleOrDefault(x => x.SourceAccountId == source && x.TargetAccountId == target && x.TypeId == 1);
            if (checkExist == null)
            {
                // Nếu không có thì không làm gì cả
                return false;
            }

            // Nếu có yêu cầu follow thì tiến hành accept follow
            string query = $"UPDATE Relationship " +
                           $"SET TypeID = 2 " +
                           $"WHERE SourceAccountId = {source} AND TargetAccountId = {target}";
            var sourceAccount = db.Accounts.SingleOrDefault(x => x.AccountId == source);
            CurrentAccount.account.Follower = CurrentAccount.account.Follower + 1;
            CurrentAccount.update();
            sourceAccount.Following = sourceAccount.Following + 1;
            db.Database.ExecuteSqlRaw(query);

			// thêm dữ liệu vào bảng Notification
			Notification newNoti = new Notification();
            newNoti.PostId = target;
			newNoti.Content = $"{CurrentAccount.account.FullName} đã chấp nhận yêu cầu theo dõi của bạn";
			newNoti.TypeNotification = 2;
			newNoti.AccountId = source;
			db.Notifications.Add(newNoti);

			db.SaveChanges();
            return true;
        }

        // Cập nhật thuộc tính isRead = true cho thông báo
        [Route("update_isRead")]
        [HttpPut]
        public bool updateIsRead(int notiId)
        {
            var noti = db.Notifications.SingleOrDefault(x => x.NotiId == notiId);
            if (noti == null)
            {
                return false;
            }
            noti.IsRead = true;
            db.SaveChanges();
            return true;
        }

    }
}
