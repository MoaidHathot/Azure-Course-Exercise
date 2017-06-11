using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeTweet.DomainModel;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeTweet.TweetsDal
{
    public sealed class TweetsRepository : ITweetsRepository
    {
        private readonly DocumentDbConfiguration _configuration;
        private DocumentClient _documentClient;
        private readonly AsyncLazy<DocumentClient> _client;
        private Uri DocumentCollectionUri { get; }
        private Uri DatabaseUri { get; }
        
        public TweetsRepository(DocumentDbConfiguration configuration)
        {
            _configuration = configuration;
            _client = new AsyncLazy<DocumentClient>(() => InitializeDocumentDbClient());

            DatabaseUri = UriFactory.CreateDatabaseUri(_configuration.DatabaseId);
            DocumentCollectionUri = UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionId);
        }

        public async Task<Tweet[]> GetAllTweetsAsync()
            => (await CreateDocumentQuery()).ToArray();

        public async Task<Tweet[]> GetTweets(string userName)
            => (await CreateDocumentQuery()).Where(tweet => tweet.Author == userName).ToArray();

        public async Task CreateTweetAsync(Tweet tweet)
            => await (await _client).CreateDocumentAsync(DocumentCollectionUri, tweet);

        private async Task<IOrderedQueryable<Tweet>> CreateDocumentQuery()
            => (await _client).CreateDocumentQuery<Tweet>(DocumentCollectionUri);

        private async Task<DocumentClient> InitializeDocumentDbClient()
        {
            _documentClient = new DocumentClient(new Uri(_configuration.EndPoint), _configuration.PrimaryKey);
            
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database {Id = _configuration.DatabaseId});
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(DatabaseUri, new DocumentCollection {Id = _configuration.CollectionId});

            return _documentClient;
        }

        public void Dispose() 
            => _documentClient?.Dispose();
    }
}
