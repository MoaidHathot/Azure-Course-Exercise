using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTweet.TweetsDal
{
    public class DocumentDbConfiguration
    {
        public string EndPoint { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; } 
       }
}
