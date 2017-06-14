using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace CodeTweet.TweetsDal
{
    internal class TweetTableStorageEntity : TableEntity
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public string ImageUri { get; set; }
    }
}
