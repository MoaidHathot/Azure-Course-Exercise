using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CodeTweet.DomainModel;
using CodeTweet.Utilities;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;


namespace CodeTweet.Queueing.ServiceBus
{
    public sealed class ServiceBusNotificationEnqueue : INotificationEnqueue, IDisposable
    {
        private readonly AsyncLazy<QueueClient> _client;
        
        private static readonly Lazy<IMapper> Mapper = new Lazy<IMapper>(CreateAutoMapper);
        
        public ServiceBusNotificationEnqueue(ServiceBusConfiguration configuration)
             => _client = new AsyncLazy<QueueClient>(() => InitializeClient(configuration));

        private static Task<QueueClient> InitializeClient(ServiceBusConfiguration configureation)
        {
            var client = new QueueClient(new ServiceBusConnectionStringBuilder(configureation.ConnectionString)
            {
                EntityPath = configureation.QueueName
            });

            return Task.FromResult(client);
        }

        public async Task EnqueueNotificationAsync(Tweet tweet) 
            => await (await _client).SendAsync(Mapper.Value.Map<Message>(tweet));

        public void Dispose()
        {
            if (_client.IsValueCreated)
            {
                _client.Value.Result
                    .CloseAsync()
                    .WithTimeout(TimeSpan.FromMilliseconds(200))
                    .Wait();
            }
        }

        private static IMapper CreateAutoMapper() 
            => new MapperConfiguration(cfg => 
                cfg.CreateMap<Tweet, Message>()
                    .ConvertUsing(tweet => new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tweet)))))
                .CreateMapper();
    }
}


