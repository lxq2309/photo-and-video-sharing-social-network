using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using SocialNetwork.Models;
using System.Text;
using static SocialNetwork.Models.CurrentAccount;

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
                        post.LikeCount--;
                        db.SaveChanges();
                        return false;
                    }
                }
                var account = db.Accounts.Include(x => x.Posts).FirstOrDefault(x => x.AccountId.Equals(CurrentAccount.account.AccountId));
                account.Posts.Add(post);
                post.LikeCount++;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        [HttpPost("addComment")]
        public IActionResult commentPost(Comment comment)
        {
            comment.AccountId = CurrentAccount.account.AccountId;
            if (ModelState.IsValid)
            {
                comment.CreateAt = DateTime.Now;
                db.Comments.Add(comment);
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

        [HttpGet("Comment")]
        public IEnumerable<Comment> GetComment(string postID)
        {
            var lstComment = db.Comments.Where(x => x.PostId == int.Parse(postID)).ToList();
            
            return lstComment;
        }
    }
}
