using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodeTweet.DomainModel;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CodeTweet.TweetsDal
{
    public sealed class DocumentDbTweetsRepository : ITweetsRepository
    {
        private readonly DocumentDbConfiguration _configuration;
        private DocumentClient _documentClient;
        private readonly AsyncLazy<DocumentClient> _client;
        private Uri DocumentCollectionUri { get; }
        private Uri DatabaseUri { get; }

        private static readonly Lazy<IMapper> Mapper = new Lazy<IMapper>(CreateMapper);
        
        public DocumentDbTweetsRepository(DocumentDbConfiguration configuration)
        {
            _configuration = configuration;
            _client = new AsyncLazy<DocumentClient>(() => InitializeDocumentDbClient());

            DatabaseUri = UriFactory.CreateDatabaseUri(_configuration.DatabaseId);
            DocumentCollectionUri = UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionId);
        }

        public async Task<Tweet[]> GetAllTweetsAsync()
            => (await CreateDocumentQuery()).ToArray().Select(tweet => Mapper.Value.Map<Tweet>(tweet)).ToArray();

        public async Task<Tweet[]> GetTweets(string userName)
            => (await CreateDocumentQuery()).Where(tweet => tweet.Author == userName).ToArray().Select(tweet => Mapper.Value.Map<Tweet>(tweet)).ToArray();

        public async Task CreateTweetAsync(Tweet tweet)
            => await (await _client).CreateDocumentAsync(DocumentCollectionUri, Mapper.Value.Map<TweetDocumentDbEntity>(tweet));

        private async Task<IOrderedQueryable<TweetDocumentDbEntity>> CreateDocumentQuery()
            => (await _client).CreateDocumentQuery<TweetDocumentDbEntity>(DocumentCollectionUri);

        private async Task<DocumentClient> InitializeDocumentDbClient()
        {
            _documentClient = new DocumentClient(new Uri(_configuration.EndPoint), _configuration.PrimaryKey);
            
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database {Id = _configuration.DatabaseId});
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(DatabaseUri, new DocumentCollection {Id = _configuration.CollectionId});

            return _documentClient;
        }

        public void Dispose() 
            => _documentClient?.Dispose();
        
        private static IMapper CreateMapper()
            => new MapperConfiguration(cfg => 
                cfg.CreateMap<Tweet, TweetDocumentDbEntity>().ReverseMap())
                .CreateMapper();
    }
}
