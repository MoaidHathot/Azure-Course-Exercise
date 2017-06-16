using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeTweet.DomainModel;
using CodeTweet.IdentityDal;
using CodeTweet.Queueing;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace CodeTweet.Notifications
{
    public class NotificationService
    {
        private readonly INotificationDequeue _dequeue;
        private readonly DbContextOptions<ApplicationIdentityContext> _identityContextOptions;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        private IDisposable _notificationSubscription = Disposable.Empty;

        public NotificationService(INotificationDequeue notificationDequeue, DbContextOptions<ApplicationIdentityContext> identityContextOptions)
        {
            _dequeue = notificationDequeue;
            _identityContextOptions = identityContextOptions;
        }

        public void Start()
        {
            _notificationSubscription = 
                _dequeue
                .Tweets
                .Subscribe(async tweet => await OnTweet(tweet));
        }

        public void Stop()
        {
            _notificationSubscription.Dispose();
            _dequeue.Dispose();
        }

        private async Task OnTweet(Tweet tweet)
        {
            try
            {
                using (var context = new ApplicationIdentityContext(_identityContextOptions))
                {
                    var repository = new UserRepository(context);
                    var users = await repository.GetUsersWithNotificationsAsync(); // Can be cached

                    foreach (var user in users)
                    {
                        SendNotification(tweet, user);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured while processing recieved tweets");
            }
        }

        private void SendNotification(Tweet tweet, string user)
            => _logger.Info($"Sent notification to user '{user}'. Tweet text: '{tweet.Text}'");
    }
}