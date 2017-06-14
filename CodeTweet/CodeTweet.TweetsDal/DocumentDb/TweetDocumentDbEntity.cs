using System;
using Newtonsoft.Json;

namespace CodeTweet.TweetsDal
{
    internal class TweetDocumentDbEntity
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime Timestamp { get; set; }
        public string ImageUri { get; set; }
    }
}