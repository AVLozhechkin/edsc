using System;
using System.Collections.Generic;
using System.IO;
using EDSc.Common.MessageBroker;
using EDSc.Common.Services;
using EDSc.Common.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace EDSc.ImgScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var rmqPublisher = new RmqPublisherBuilder()
                .WithExchangeAutoCreation()
                .UsingConfigExchangeAndRoutingKey(configuration.GetSection("RmqPublisher"))
                .UsingCustomHost("localhost")
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(rmqPublisher)
                .AddSingleton<IDataRetriever, HttpDataRetriever>()
                .AddSingleton<IJob, ImgScrapingJob>()
                .AddSingleton<IImgDownloadingService, RedditImgDownloadingService>(x =>
                    new RedditImgDownloadingService(
                        x.GetService<IDataRetriever>()))
                .AddSingleton(
                    x => new QuartzTaskManager<ImgScrapingJob>(
                        new Dictionary<string, object>
                        {
                            {"IRmqPublisher", x.GetService<IRmqPublisher>() },
                            {"IImgDownloadingService", x.GetService<IImgDownloadingService>() }
                        }, new TimeSpan(1, 0, 0)
                        ))
                .BuildServiceProvider();

            var taskManager = serviceProvider.GetService<QuartzTaskManager<ImgScrapingJob>>();


            taskManager.Start().Wait();
        }
    }
}
