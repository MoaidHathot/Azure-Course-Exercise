using System;
using System.IO;
using CodeTweet.IdentityDal;
using CodeTweet.Notifications;
using CodeTweet.Queueing.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace CodeTweet.Worker
{
    class Program
    {
        public static void Main()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = configurationBuilder.Build();

            ServiceRunner<NotificationServiceWrapper>.Run(config =>
            {
                var name = config.GetDefaultName();
                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory(extraArguments =>
                    {
                        var zeroConfiguration = new ServiceBusConfiguration();
                        configuration.GetSection("ServiceBus").Bind(zeroConfiguration);
                        var notificationDequeue = new ServiceBusNotificationDequeue(zeroConfiguration);

                        var identityContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityContext>();
                        identityContextOptionsBuilder.UseSqlServer(configuration.GetConnectionString("Identity"));

                        var notificationService = new NotificationService(notificationDequeue, identityContextOptionsBuilder.Options);
                        return new NotificationServiceWrapper(notificationService);
                    });
                    serviceConfig.OnStart((service, extraArguments) =>
                    {
                        Console.WriteLine("Service {0} started", name);
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        Console.WriteLine("Service {0} stopped", name);
                        service.Stop();
                    });

                    serviceConfig.OnError(e =>
                    {
                        Console.WriteLine("Service {0} errored with exception : {1}", name, e.Message);
                    });
                });
            });

            Console.WriteLine("Press <ENTER> to exit...");
            Console.ReadLine();
        }

        class NotificationServiceWrapper : MicroService, IMicroService
        {
            private readonly NotificationService _inner;

            public NotificationServiceWrapper(NotificationService inner) 
                => _inner = inner;

            public void Start()
                => _inner.Start();

            public void Stop() 
                => _inner.Stop();
        }
    }
}