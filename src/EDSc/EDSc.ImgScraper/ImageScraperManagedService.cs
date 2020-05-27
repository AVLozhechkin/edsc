using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EDSc.Common.MessageBroker;
using EDSc.Common.Services;
using EDSc.Common.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.AspNetCore.Configuration;
using Microsoft.ServiceFabric.Services.Runtime;
using Quartz;

namespace EDSc.ImgScraper
{
    class ImageScraperManagedService : StatelessService
    {
        public ImageScraperManagedService(StatelessServiceContext context) : base(context)
        {

        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var builder = new ConfigurationBuilder()
                .AddServiceFabricConfiguration();

                var configuration = builder.Build();

                var rmqPublisher = new RmqPublisherBuilder()
                    .UsingConfigExchangeAndRoutingKey(configuration.GetSection("Config").GetSection("Binding"))
                    .UsingCustomHost("localhost")
                    .Build();

                var cronInterval = configuration.GetSection("Config").GetSection("QuartzManager").GetSection("CronInterval").Value;

                var serviceProvider = new ServiceCollection()
                    .AddSingleton(rmqPublisher)
                    .AddSingleton<IDataRetriever, HttpDataRetriever>()
                    .AddSingleton<IJob, ImgScrapingJob>()
                    .AddSingleton<IImgDownloadingService, RedditImgDownloadingService>(x =>
                        new RedditImgDownloadingService(
                            configuration.GetSection("Config").GetSection("ImgDownloadingService"),
                            x.GetService<IDataRetriever>()))
                    .AddSingleton(
                        x => new QuartzTaskManager<ImgScrapingJob>(
                            new Dictionary<string, object>
                            {
                            {"IRmqPublisher", x.GetService<IRmqPublisher>() },
                            {"IImgDownloadingService", x.GetService<IImgDownloadingService>() }
                            }, cronInterval))
                    .BuildServiceProvider();

                var taskManager = serviceProvider.GetService<QuartzTaskManager<ImgScrapingJob>>();


                taskManager.Start();
            });
        }
    }
}
