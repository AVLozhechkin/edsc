using System;
using System.IO;
using EDSc.Common.MessageBroker;
using EDSc.Common.Model;
using EDSc.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;

namespace EDSc.ImageClassifier
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
                .UsingConfigExchangeAndRoutingKey(configuration.GetSection("RmqPublisher"))
                .UsingCustomHost("localhost")
                .Build();
            var rmqConsumer = new RmqConsumerBuilder()
                .UsingQueue("ImageClassifier")
                .UsingCustomHost("localhost")
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(rmqPublisher)
                .AddSingleton(rmqConsumer)
                .AddPredictionEnginePool<InMemoryImage, ImagePrediction>().FromFile("imageClassifier.zip")
                .Services.BuildServiceProvider();
            
            var serv = new ImageClassificationService(
                serviceProvider.GetService<IRmqConsumer>(),
                serviceProvider.GetService<IRmqPublisher>(),
                serviceProvider.GetService<PredictionEnginePool<InMemoryImage, ImagePrediction>>());

            serv.Start();
        }
    }
}