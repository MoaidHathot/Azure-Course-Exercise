using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace CodeTweet.Worker.WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([ServiceBusTrigger("Tweets")] Tweet tweet, TextWriter log)
        {
            //log.WriteLine(message);
            log.WriteLine($"A new tweet was tweeted by {tweet.Author} on {tweet.Timestamp}. Text: {tweet.Text}.{(string.IsNullOrWhiteSpace(tweet.ImageUri) ? "" : $"With an image: {tweet.ImageUri}")}");
        }
    }

    public class Tweet
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime Timestamp { get; set; }
        public string ImageUri { get; set; }
    }
}
