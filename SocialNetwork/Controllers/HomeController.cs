using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;
using SocialNetwork.Models.Authentication;
using System.Diagnostics;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace SocialNetwork.Controllers
{
	public class HomeController : Controller
	{
		private IHostingEnvironment _env;
		private readonly ILogger<HomeController> _logger;
		private SocialNetworkDbContext context = new SocialNetworkDbContext();

		public HomeController(ILogger<HomeController> logger, IHostingEnvironment _enviroment)
		{
			_logger = logger;
			_env = _enviroment;
		}

		[Authentication]
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		//POST OPTION
		[HttpPost]
		public IActionResult CreatePost(string Content, List<IFormFile> images)
		{
			Post post = new Post();
			post.AccountId = CurrentAccount.account.AccountId;
			post.Content = Content;
			post.CreateAt = DateTime.Now;
			post.LikeCount = 0;
			post.CommentCount = 0;
			post.IsDeleted = false;
			context.Posts.Add(post);
			context.SaveChanges();
			foreach (var image in images)
			{
				if (image != null)
				{
					string serverMapPath = Path.Combine(_env.WebRootPath, $"images/post/{CurrentAccount.account.AccountId}/{post.PostId}");
					string serverMapPathFile = Path.Combine(serverMapPath, image.FileName);
					Directory.CreateDirectory(serverMapPath);
					using (var stream = new FileStream(serverMapPathFile, FileMode.Create))
					{
						image.CopyTo(stream);
					}
					string filepath = $"/images/post/{CurrentAccount.account.AccountId}/{post.PostId}/{image.FileName}";
					Medium medium = new Medium();
					medium.PostId = post.PostId;
					medium.MediaLink = filepath.ToString();
                    if (image.ContentType.Contains("image"))
                    {
                        medium.MediaType = "image";
                    }
					else if (image.ContentType.Contains("video"))
					{
						medium.MediaType = "video";
					}
					context.Media.Add(medium);
				}
			}
			context.SaveChanges();
			return RedirectToAction("Index");
		}

        public IActionResult DeletePost(string postId)
		{
            Post post = context.Posts.SingleOrDefault(x => x.PostId.ToString() == postId);
            if (post != null)// && CurrentAccount.account.AccountId == post.AccountId)
			{
                post.IsDeleted = true;
                context.Entry(post).State = EntityState.Modified;
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

		[Route("~/p/{postId}")]
		public IActionResult SinglePostDetail(int postId)
		{
            var singlePost = context.Posts.SingleOrDefault(x => x.PostId == postId && x.IsDeleted == false);
			if (singlePost == null)
			{
				return RedirectToAction("Index");
			}
			var singlePostDetail = new ViewModels.PostDetailViewModel(singlePost);
			return View(singlePostDetail);
        }
    }
}