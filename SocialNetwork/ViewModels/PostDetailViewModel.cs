using SocialNetwork.Models;

namespace SocialNetwork.ViewModels
{
    public class PostDetailViewModel
    {
        SocialNetworkDbContext _context = new SocialNetworkDbContext();
        public Post Post { get; set; }

        public PostDetailViewModel(Post post)
        {
            Post = post;
        }

        public Account GetPostOwnerAccount()
        {
            return _context.Accounts.Single(x => x.AccountId== Post.AccountId);
        }

        public List<Medium> GetListMedia ()
        {
            return _context.Media.Where(x => x.PostId == Post.PostId).ToList();
        }

        public List<Comment> GetListComment()
        {
            return _context.Comments.Where(x => x.PostId == Post.PostId).ToList();
        }

        public Account GetCommentAccount(Comment comment)
        {
            return _context.Accounts.Single(x => x.AccountId == comment.AccountId);
        }

    }
}
