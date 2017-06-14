using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeTweet.DomainModel;
using CodeTweet.IdentityDal.Model;
using CodeTweet.ImagesDal;
using CodeTweet.Queueing;
using CodeTweet.TweetsDal;
using CodeTweet.Web.Models.TweetsViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Documents;

namespace CodeTweet.Web.Managers
{
    public class TweetsManager
    {
        private readonly ITweetsRepository _tweetsRepository;
        private readonly IImagesRepository _imagesRepository;
        private readonly INotificationEnqueue _queue;
        private readonly UserManager<ApplicationUser> _userManager;

        public TweetsManager(ITweetsRepository tweetsRepository, IImagesRepository imagesRepository,
            INotificationEnqueue queue, UserManager<ApplicationUser> userManager)
        {
            _tweetsRepository = tweetsRepository;
            _imagesRepository = imagesRepository;
            _queue = queue;
            _userManager = userManager;
        }

        public async Task<Tweet[]> GetAllTweetsAsync()
            => await _tweetsRepository.GetAllTweetsAsync();

        public async Task<Tweet[]> GetTweets(ClaimsPrincipal user)
            => await _tweetsRepository.GetTweets(_userManager.GetUserName(user));

        public async Task CreateTweetAsync(ClaimsPrincipal user, NewTweetViewModel tweetViewModel)
        {
            var tweetId = Guid.NewGuid();

            var imageUrl = null != tweetViewModel.Image
                ? await UploadImage(tweetId.ToString(), tweetViewModel.Image)
                : string.Empty;

            var tweet = new Tweet
            {
                Id = tweetId,
                Author = _userManager.GetUserName(user),
                Text = tweetViewModel?.Text ?? string.Empty,
                Timestamp = DateTime.UtcNow,
                ImageUri = imageUrl
            };

            await _tweetsRepository.CreateTweetAsync(tweet);
            await _queue.EnqueueNotificationAsync(tweet);
        }

        private async Task<string> UploadImage(string id, IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                return await _imagesRepository.UploadImageAsync(id, stream);
            }
        }
    }
}
