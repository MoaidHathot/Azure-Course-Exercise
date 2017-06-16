using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace CodeTweet.Queueing.ServiceBus
{
    public static class ObservableExtensions
    {
        public static IObservable<Message> ObserveReceivedMessages(this QueueClient client)
        {
            return Observable.Create<Message>(observer =>
            {
                client.RegisterMessageHandler((message, cancellation) =>
                {
                    observer.OnNext(message);
                    return Task.CompletedTask;
                });

                return observer.OnCompleted;
            });
        }
    }
}
