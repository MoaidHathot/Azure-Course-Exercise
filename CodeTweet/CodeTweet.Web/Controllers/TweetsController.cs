using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CodeTweet.DomainModel;
using CodeTweet.IdentityDal.Model;
using CodeTweet.Queueing;
using CodeTweet.TweetsDal;
using CodeTweet.Web.Models.TweetsViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CodeTweet.Web.Controllers
{
    [Authorize]
    public class TweetsController : Controller
    {
        private readonly ITweetsRepository _repository;
        private readonly INotificationEnqueue _queue;
        private readonly UserManager<ApplicationUser> _userManager;

        public TweetsController(ITweetsRepository repository, INotificationEnqueue queue, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _queue = queue;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            return View(await _repository.GetAllTweetsAsync());
        }

        public async Task<ActionResult> Me()
        {
            string userName = _userManager.GetUserName(User);
            return View(await _repository.GetTweets(userName));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Text", "Image")] NewTweetViewModel newTweet)
        {
            if (ModelState.IsValid)
            {
                var tweet = new Tweet
                {
                    Id = Guid.NewGuid(),
                    Author = _userManager.GetUserName(User),
                    Text = newTweet.Text,
                    Timestamp = DateTime.UtcNow
                };
                await _repository.CreateTweetAsync(tweet);
                await _queue.EnqueueNotificationAsync(tweet);
                return RedirectToAction("Index");
            }

            return View(newTweet);
        }
    }
}
