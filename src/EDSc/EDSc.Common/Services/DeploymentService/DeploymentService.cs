namespace EDSc.Common.Services.Deployment
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using EDSc.Common.MessageBroker;
    using EDSc.Common.Services.Deployment.Model;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;

    public class DeploymentService
    {
        private readonly string ServiceName = "DeploymentService";

        private readonly IServiceDescriptionRepository serviceDescriptionRepository;
        private readonly IEnumerable<IDeploymentStrategy> deploymentStrategies;

        public DeploymentService(IRmqConsumer rmqConsumer, IServiceDescriptionRepository serviceDescriptionRepository, IEnumerable<IDeploymentStrategy> deploymentStrategies)
        {
            this.RmqConsumer = rmqConsumer;
            this.serviceDescriptionRepository = serviceDescriptionRepository;
            this.deploymentStrategies = deploymentStrategies;
        }

        public IRmqConsumer RmqConsumer { get; }

        public bool CanExecute(string name)
        {
            return (ServiceName == name);
        }

        public void Start()
        {
            this.RmqConsumer.StartListening(this.OnReceive);
        }

        public void Stop()
        {
            this.RmqConsumer.StopListening();
        }

        private async void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            var deploymentDescription = JsonConvert.DeserializeObject<DeploymentDescription>(Encoding.UTF8.GetString(e.Body));

            var serviceDescription = await serviceDescriptionRepository.GetServiceDescriptionByComponentIdAsync(int.Parse(deploymentDescription.ApplicationInstanceId));

            await this.deploymentStrategies.Single(s => s.CanExecute(deploymentDescription.DeploymentType)).ProcessDeployment(serviceDescription);
            this.RmqConsumer.Ack(new BasicDeliverEventArgs() { DeliveryTag = e.DeliveryTag });
        }
    }
}