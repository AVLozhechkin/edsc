namespace EDSc.Common.Services.Deployment
{
    using EDSc.Common.Utils.MessageBroker;
    using EDSc.Common.Services.Deployment.Database;
    using EDSc.Common.Services.Deployment.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;

    public class DeploymentService
    {
        private IInstanceDescriptionRepository InstanceDescriptionRepository { get; }
        private IEnumerable<IDeploymentStrategy> DeploymentStrategies { get; }
        private object locker = new object();

        public DeploymentService(IRmqConsumer rmqConsumer, 
            IInstanceDescriptionRepository instanceDescriptionRepository, 
            IEnumerable<IDeploymentStrategy> deploymentStrategies)
        {
            this.RmqConsumer = rmqConsumer;
            this.InstanceDescriptionRepository = instanceDescriptionRepository;
            this.DeploymentStrategies = deploymentStrategies;
        }

        private IRmqConsumer RmqConsumer { get; }

        public void Start()
        {
            this.RmqConsumer.StartListening(this.OnReceive);
        }

        public void Stop()
        {
            this.RmqConsumer.StopListening();
        }

        private void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            lock (locker)
            {
                var deploymentDescription = JsonConvert
                    .DeserializeObject<DeploymentDescription>(Encoding.UTF8.GetString(e.Body));

                var serviceDescription = InstanceDescriptionRepository
                    .GetInstanceDescriptionByIdAsync(int.Parse(deploymentDescription.ApplicationInstanceId)).Result; 
                this.DeploymentStrategies
                    .Single(s => 
                        s.CanExecute(deploymentDescription.DeploymentType))
                    .ProcessDeployment(serviceDescription).Wait();
                this.RmqConsumer.Ack(e);
            }
        }
    }
}