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
            string serializedTweet;
            if (!_client.TryReceiveFrameString(TimeSpan.FromSeconds(1), out serializedTweet))
                return new Tweet[0];

            var tweet = JsonConvert.DeserializeObject<Tweet>(serializedTweet);
            return new[] {tweet};
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}