namespace CodeTweet.TweetsDal
{
    public sealed class DocumentDbConfiguration
    {
        public string EndPoint { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; } 
       }
}
