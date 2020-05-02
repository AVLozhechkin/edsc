using EDSc.Common.MessageBroker;
using EDSc.Common.Model;
using EDSc.Common.Services;
using EDSc.Common.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.IO;

namespace EDSc.ImageSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            var consumerQueue = configuration.GetSection("RmqConsumer").GetValue<string>("ReceiverQueue");
            var client = new MongoClient(configuration.GetConnectionString("ImageDb"));

            var rmqConsumer = new RmqConsumerBuilder()
                .UsingQueue(consumerQueue)
                .UsingCustomHost("localhost")
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(rmqConsumer)
                .AddSingleton<IImgToDbWriter<InMemoryImage>>(e => new ImgToMongoWriter(configuration.GetSection("ImageSaver"), client))
                .BuildServiceProvider();

            var saver = new ImgSavingService(
                serviceProvider.GetService<IImgToDbWriter<InMemoryImage>>(),
                serviceProvider.GetService<IRmqConsumer>());
            saver.Start();
        }
    }
}
