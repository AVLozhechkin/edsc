using EDSc.Common.MessageBroker;
using EDSc.Common.Model;
using EDSc.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using Microsoft.ServiceFabric.AspNetCore.Configuration;
using Microsoft.ServiceFabric.Services.Runtime;
using MongoDB.Bson;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EDSc.ImageClassifier
{
    class ImageClassifierManagedService : StatelessService
    {
        public ImageClassifierManagedService(StatelessServiceContext context)
          : base(context)
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
                    .UsingConfigExchangeAndRoutingKey(configuration.GetSection("Config:Binding"))
                    .UsingCustomHost("localhost")
                    .Build();
                configuration.ToJson();
                var queue = configuration.GetSection("Config").GetValue<string>("Binding:ReceiverQueue");
                File.WriteAllText("D:\\log.txt", queue);
                var rmqConsumer = new RmqConsumerBuilder()
                    .UsingQueue(queue)
                    .UsingCustomHost("localhost")
                    .Build();
                var serviceProvider = new ServiceCollection()
                    .AddSingleton(rmqPublisher)
                    .AddSingleton(rmqConsumer)
                    .AddPredictionEnginePool<InMemoryImage, ImagePrediction>().FromFile(
                        Path.Combine(
                            FabricRuntime.GetActivationContext().GetCodePackageObject("Code").Path, "imageClassifier.zip"))
                    .Services.BuildServiceProvider();

                var serv = new ImageClassificationService(
                    serviceProvider.GetService<IRmqConsumer>(),
                    serviceProvider.GetService<IRmqPublisher>(),
                    serviceProvider.GetService<PredictionEnginePool<InMemoryImage, ImagePrediction>>());

                serv.Start();
            });
        }
    }
}
