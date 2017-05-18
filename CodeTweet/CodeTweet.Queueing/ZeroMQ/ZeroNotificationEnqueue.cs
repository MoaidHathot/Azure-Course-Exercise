using System;
using System.Threading;
using System.Threading.Tasks;
using CodeTweet.DomainModel;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace CodeTweet.Queueing.ZeroMQ
{
    public sealed class ZeroNotificationEnqueue : INotificationEnqueue, IDisposable
    {
        private readonly NetMQPoller _poller;
        private readonly PushSocket _client;

        public ZeroNotificationEnqueue(ZeroConfiguration configuration)
        {
            _poller = new NetMQPoller();
            _client = new PushSocket();
            _client.Connect(configuration.ZeroMqAddress);
            _poller.RunAsync();
        }

        public Task EnqueueNotificationAsync(Tweet tweet)
        {
            var serializedObject = JsonConvert.SerializeObject(tweet);
            return Task.Factory.StartNew(() => _client.TrySendFrame(TimeSpan.FromSeconds(1), serializedObject), CancellationToken.None, TaskCreationOptions.None, _poller);
        }

        public void Dispose()
        {
            _poller.Stop();
            _poller.Dispose();
            _client.Dispose();
        }
    }
}