using System.ComponentModel.DataAnnotations;
using CodeTweet.Web.Utilities;
using Microsoft.AspNetCore.Http;

namespace CodeTweet.Web.Models.TweetsViewModels
{
    public class NewTweetViewModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(140)]
        public string Text { get; set; }

        [PostedFileExtensions(Extensions = "jpg,jpeg,png,gif")]
        public IFormFile Image { get; set; }
    }
}