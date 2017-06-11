using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CodeTweet.Web.Models.TweetsViewModels
{
    public class NewTweetViewModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(140)]
        public string Text { get; set; }

        [FileExtensions(Extensions = "jpg,jpeg,png")]
        public IFormFile Image { get; set; }
    }
}