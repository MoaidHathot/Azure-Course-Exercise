using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CodeTweet.DomainModel;
using CodeTweet.Utilities;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace CodeTweet.Queueing.ServiceBus
{
    public sealed class ServiceBusNotificationDequeue : INotificationDequeue
    {
        private readonly Lazy<QueueClient> _client;

        private static readonly Lazy<IMapper> Mapper = new Lazy<IMapper>(CreateAutoMapper);
        
        public IObservable<Tweet> Tweets { get; }

        public ServiceBusNotificationDequeue(ServiceBusConfiguration configuration)
        {
            _client = new Lazy<QueueClient>(() => InitializeClient(configuration));

            Tweets = Observable.Defer(() => _client.Value.ObserveReceivedMessages())
                    .Select(message => Mapper.Value.Map<Tweet>(message))
                    .Publish()
                    .RefCount();
        }

        private static QueueClient InitializeClient(ServiceBusConfiguration configureation)
            => new QueueClient(new ServiceBusConnectionStringBuilder(configureation.ConnectionString)
            {
                EntityPath = configureation.QueueName
            });

        public void Dispose()
        {
            if (_client.IsValueCreated)
            {
                _client.Value
                    .CloseAsync()
                    .WithTimeout(TimeSpan.FromMilliseconds(200))
                    .Wait();
            }
        }

        private static IMapper CreateAutoMapper()
            => new MapperConfiguration(cfg =>
                    cfg.CreateMap<Message, Tweet>()
                        .ConvertUsing(message => JsonConvert.DeserializeObject<Tweet>(Encoding.ASCII.GetString(message.Body))))
                .CreateMapper();
    }
}
