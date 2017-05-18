using System.ComponentModel.DataAnnotations;

namespace CodeTweet.Web.Models.TweetsViewModels
{
    public class NewTweetViewModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(140)]
        public string Text { get; set; }
    }
}