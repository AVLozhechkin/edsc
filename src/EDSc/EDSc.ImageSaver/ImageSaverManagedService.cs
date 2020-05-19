using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using EDSc.Common.MessageBroker;
using EDSc.Common.Model;
using EDSc.Common.Services;
using EDSc.Common.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.AspNetCore.Configuration;
using Microsoft.ServiceFabric.Services.Runtime;
using MongoDB.Driver;

namespace EDSc.ImageSaver
{
    class ImageSaverManagedService : StatelessService
    {
        public ImageSaverManagedService(StatelessServiceContext context) : base(context)
        {

        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var builder = new ConfigurationBuilder()
                .AddServiceFabricConfiguration();

                var configuration = builder.Build();

                var queue = configuration.GetSection("Config").GetValue<string>("Binding:ReceiverQueue");
                var connString = configuration.GetSection("Config").GetValue<string>("ConnectionStrings:Mongo");

                var client = new MongoClient(connString);

                var rmqConsumer = new RmqConsumerBuilder()
                    .UsingQueue(queue)
                    .UsingCustomHost("localhost")
                    .Build();

                var serviceProvider = new ServiceCollection()
                    .AddSingleton(rmqConsumer)
                    .AddSingleton<IImgToDbWriter<InMemoryImage>>(e => new ImgToMongoWriter(configuration.GetSection("Config").GetSection("Db"), client))
                    .BuildServiceProvider();

                var saver = new ImgSavingService(
                    serviceProvider.GetService<IImgToDbWriter<InMemoryImage>>(),
                    serviceProvider.GetService<IRmqConsumer>());
                saver.Start();
            });
        }
    }
}
