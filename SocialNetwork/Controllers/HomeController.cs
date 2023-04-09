using Microsoft.AspNetCore.Mvc;
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
			var lst = context.Posts.ToList();
			return View(lst);
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
			Medium medium = new Medium();
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
					string serverMapPath = Path.Combine(_env.WebRootPath, "images/post/" + CurrentAccount.account.AccountId);
					string serverMapPathFile = Path.Combine(serverMapPath, image.FileName);
					Directory.CreateDirectory(serverMapPath);
					using (var stream = new FileStream(serverMapPathFile, FileMode.Create))
					{
						image.CopyTo(stream);
					}
					string filepath = "images/post/" + CurrentAccount.account.AccountId + "/" + image.FileName;
					var postID = post.PostId;
					medium.PostId = post.PostId;
					medium.MediaLink = filepath.ToString();
					medium.MediaType = image.GetType().Name.ToString();
					context.Media.Add(medium);
				}
			}
			context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}