using System;
using System.Collections.Generic;
using System.IO;
using EDSc.Common.MessageBroker;
using EDSc.Common.Services.Deployment;
using EDSc.Common.Services.DeploymentService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace EDSc.DeploymentService
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
            var client = new MongoClient(configuration.GetConnectionString("Db"));
            var rmqConsumer = new RmqConsumerBuilder()
                .UsingQueue(consumerQueue)
                .UsingCustomHost("localhost")
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(rmqConsumer)
                .AddSingleton(x => new ServiceFabricManager(configuration.GetSection("ServiceFabric")))
                .AddSingleton<IConfigConverter<string, Dictionary<string, string>>, JsonToDictionaryConfigConverter>()
                .AddTransient<IPackageManager, FilePackageManager>(x => new FilePackageManager(configuration.GetSection("PackageManager")))
                .AddSingleton<IServiceDescriptionRepository, MongoServiceDescriptionRepository>(
                    x => new MongoServiceDescriptionRepository(configuration.GetSection("Mongo"), client))
                .AddSingleton<IDeploymentStrategy, DeployingServiceStrategy>()
                .AddSingleton<IDeploymentStrategy, RemovingServiceStrategy>()
                .BuildServiceProvider();

            var depService = new Common.Services.Deployment.DeploymentService(
                serviceProvider.GetService<IRmqConsumer>(),
                serviceProvider.GetService<IServiceDescriptionRepository>(),
                serviceProvider.GetServices<IDeploymentStrategy>());
            depService.Start();
            Console.WriteLine("Hello");
        }
    }
}
