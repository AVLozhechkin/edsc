namespace EDSc.DeploymentService
{
    using System.Collections.Generic;
    using System.IO;
    using EDSc.Common.Services.Deployment;
    using EDSc.Common.Services.Deployment.Database;
    using EDSc.Common.Services.Deployment.Strategies;
    using EDSc.Common.Services.Deployment.Util;
    using EDSc.Common.Utils.MessageBroker;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    
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
                .AddSingleton(x => rmqConsumer)
                .AddSingleton<IConfigConverter<string, Dictionary<string, string>>, JsonToDictionaryConfigConverter>()
                .AddSingleton<IPackageManager, FilePackageManager>(x => 
                    new FilePackageManager(configuration.GetSection("PackageManager")))
                .AddSingleton<IInstanceDescriptionRepository, MongoInstanceDescriptionRepository>(
                    x => new MongoInstanceDescriptionRepository(
                        configuration.GetSection("Mongo"), client))
                .AddSingleton(x => new ServiceFabricManager(
                    configuration.GetSection("ServiceFabric")))
                .AddSingleton<IDeploymentStrategy, DeployingStrategy>()
                .AddSingleton<IDeploymentStrategy, RemovingStrategy>()
                .BuildServiceProvider();

            var depService = new DeploymentService(
                serviceProvider.GetService<IRmqConsumer>(),
                serviceProvider.GetService<IInstanceDescriptionRepository>(),
                serviceProvider.GetServices<IDeploymentStrategy>());
            depService.Start();
        }
    }
}
