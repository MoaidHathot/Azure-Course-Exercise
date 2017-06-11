using System;
using CodeTweet.DomainModel;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace CodeTweet.Queueing.ZeroMQ
{
    public sealed class ZeroNotificationDequeue : INotificationDequeue
    {
        private readonly PullSocket _client;

        public ZeroNotificationDequeue(ZeroConfiguration configuration)
        {
            _client = new PullSocket();
            _client.Bind(configuration.ZeroMqAddress);
        }

        public Tweet[] Dequeue()
        {
            if (!_client.TryReceiveFrameString(TimeSpan.FromSeconds(1), out string serializedTweet))
            {
                return new Tweet[0];
            }

            return new[] { JsonConvert.DeserializeObject<Tweet>(serializedTweet) };
        }

        public void Dispose() 
            => _client.Dispose();
    }
}