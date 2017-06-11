using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeTweet.TweetsDal.Model
{
    public class TweetEntity
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime Timestamp { get; set; }
    }
}