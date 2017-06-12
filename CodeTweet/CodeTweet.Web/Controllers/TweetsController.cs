using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CodeTweet.DomainModel;
using CodeTweet.IdentityDal.Model;
using CodeTweet.ImagesDal;
using CodeTweet.Queueing;
using CodeTweet.TweetsDal;
using CodeTweet.Web.Managers;
using CodeTweet.Web.Models.TweetsViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CodeTweet.Web.Controllers
{
    [Authorize]
    public class TweetsController : Controller
    {
        private readonly TweetsManager _manager;

        public TweetsController(TweetsManager manager)
        {
            _manager = manager;
        }

        public async Task<ActionResult> Index() 
            => View(await _manager.GetAllTweetsAsync());

        public async Task<ActionResult> Me() 
            => View(await _manager.GetTweets(User));

        public ActionResult Create() 
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Text", "Image")] NewTweetViewModel newTweet)
        {
            if (ModelState.IsValid)
            {
                await _manager.CreateTweetAsync(User, newTweet);
                
                return RedirectToAction("Index");
            }

            return View(newTweet);
        }
    }
}
