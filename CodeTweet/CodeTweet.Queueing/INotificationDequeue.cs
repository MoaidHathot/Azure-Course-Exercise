using System;
using CodeTweet.DomainModel;

namespace CodeTweet.Queueing
{
    public interface INotificationDequeue : IDisposable
    {
        IObservable<Tweet> Tweets { get; }
    }
}