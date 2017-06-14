using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CodeTweet.DomainModel;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CodeTweet.TweetsDal
{
    public class TableStorageTweetsRepository : ITweetsRepository
    {
        private readonly AsyncLazy<CloudTable> _table;
        
        private static readonly Lazy<IMapper> Mapper = new Lazy<IMapper>(CreateMapper);
        
        public TableStorageTweetsRepository(TableStorageConfiguration configuration) 
            => _table = new AsyncLazy<CloudTable>(() => InitializeTableStorageClient(configuration));

        public async Task<Tweet[]> GetAllTweetsAsync()
            => (await (await _table)
                .ExecuteQuerySegmentedAsync(new TableQuery<TweetTableStorageEntity>(), new TableContinuationToken()))
                .Results
                .Select(tweetEntry => Mapper.Value.Map<Tweet>(tweetEntry))
                .ToArray();

        public async Task<Tweet[]> GetTweets(string userName) 
            => (await (await _table)
                .ExecuteQuerySegmentedAsync(new TableQuery<TweetTableStorageEntity>().Where(TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, userName)), new TableContinuationToken()))
                .Results
                .Select(tweetEntry => Mapper.Value.Map<Tweet>(tweetEntry))
                .ToArray();

        public async Task CreateTweetAsync(Tweet tweet)
        {
            var mapped = Mapper.Value.Map<TweetTableStorageEntity>(tweet);
            var operation = TableOperation.Insert(mapped);
            await (await _table).ExecuteAsync(operation);
        }

        private async Task<CloudTable> InitializeTableStorageClient(TableStorageConfiguration configuration)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("Tweets");

            await table.CreateIfNotExistsAsync();

            return table;
        }

        public void Dispose()
        {
            
        }

        private static IMapper CreateMapper()
            => new MapperConfiguration(cfg =>
                    cfg.CreateMap<Tweet, TweetTableStorageEntity>()
                        .ForMember(tweet => tweet.RowKey, operation => operation.MapFrom(source => source.Id))
                        .ForMember(tweet => tweet.PartitionKey, operation => operation.MapFrom(source => source.Author))
                        .ReverseMap()
                        .ForMember(tweet => tweet.Timestamp, operation => operation.MapFrom(source => source.Timestamp.DateTime)))
                .CreateMapper();
    }
}
