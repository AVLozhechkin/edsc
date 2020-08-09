namespace EDSc.ImageScraper
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using EDSc.Common.Services.Scraping;
    using EDSc.Common.Services.Scraping.Utils;
    using EDSc.Common.Utils.MessageBroker;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.AspNetCore.Configuration;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Quartz;
    
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
                    .UsingConfigExchangeAndRoutingKey(configuration.GetSection("Config")
                        .GetSection("Binding"))
                    .UsingCustomHost("localhost")
                    .Build();

                var cronInterval = configuration.GetSection("Config")
                    .GetSection("QuartzManager")
                    .GetSection("CronInterval").Value;

                var serviceProvider = new ServiceCollection()
                    .AddSingleton(rmqPublisher)
                    .AddSingleton<IDataRetriever, HttpDataRetriever>()
                    .AddSingleton<IJob, ImageScrapingJob>()
                    .AddSingleton<IImageDownloadingService, RedditImageDownloadingService>(x =>
                        new RedditImageDownloadingService(
                            configuration.GetSection("Config").GetSection("ImageDownloadingService"),
                            x.GetService<IDataRetriever>()))
                    .AddSingleton(
                        x => new QuartzTaskManager<ImageScrapingJob>(
                            new Dictionary<string, object>
                            {
                            {"IRmqPublisher", x.GetService<IRmqPublisher>() },
                            {"IImageDownloadingService", x.GetService<IImageDownloadingService>() }
                            }, cronInterval))
                    .BuildServiceProvider();

                var taskManager = serviceProvider.GetService<QuartzTaskManager<ImageScrapingJob>>();


                taskManager.Start();
            }, cancellationToken);
        }
    }
}
