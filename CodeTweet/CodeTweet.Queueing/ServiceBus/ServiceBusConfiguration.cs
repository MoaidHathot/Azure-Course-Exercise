using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTweet.Queueing.ServiceBus
{
    public sealed class ServiceBusConfiguration
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
