namespace EDSc.Common.Tests.Services.Deployment
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EDSc.Common.Services.Deployment;
    using EDSc.Common.Services.Deployment.Database;
    using EDSc.Common.Services.Deployment.Model;
    using EDSc.Common.Utils.MessageBroker;
    using Newtonsoft.Json;
    using NSubstitute;
    using NUnit.Framework;
    using RabbitMQ.Client.Events;
    
    [TestFixture]
    public class DeploymentServiceTests
    {
        [Test]
        public void ShouldDeployAndAck()
        {
            // Arrange
            var descriptionRepository = Substitute.For<IInstanceDescriptionRepository>();
            var instanceDescription = new InstanceDescription
            {
                ServiceId = "1",
                ApplicationName = "TestApp",
                ApplicationTypeName = "TestType"
            };
            descriptionRepository.GetInstanceDescriptionByIdAsync(Arg.Is(1)).Returns(instanceDescription);
            var deploymentDescription = new DeploymentDescription
            {
                DeploymentType = "deploy", ApplicationInstanceId = "1"
            };
            var eventArgs = new BasicDeliverEventArgs
            {
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deploymentDescription))
            };
            var consumer = Substitute.For<IRmqConsumer>();
            consumer
                .When(w => w.StartListening(
                    Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                .Do((args) =>
                {
                    args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, eventArgs);
                });
            
            var deploymentStrategy = Substitute.For<IDeploymentStrategy>();
            deploymentStrategy.CanExecute(Arg.Is("deploy")).Returns(true);
            var removingStrategy = Substitute.For<IDeploymentStrategy>();
            removingStrategy.CanExecute(Arg.Is("remove")).Returns(true);
            var strategies = new List<IDeploymentStrategy> { deploymentStrategy, removingStrategy };
            
            var sut = new DeploymentService(consumer, descriptionRepository, strategies);
            
            // Act
            
            sut.Start();
            
            // Assert
            deploymentStrategy.Received(1).CanExecute(Arg.Is("deploy"));
            deploymentStrategy.Received(1)
                .ProcessDeployment(
                    Arg.Is<InstanceDescription>(inDesc => 
                        inDesc.ApplicationName == instanceDescription.ApplicationName &&
                        inDesc.ApplicationTypeName == instanceDescription.ApplicationTypeName));
            consumer.Received(1).Ack(eventArgs);
        }

        [Test]
        public void ShouldRemoveAndAck()
        {
            // Arrange
            var descriptionRepository = Substitute.For<IInstanceDescriptionRepository>();
            var instanceDescription = new InstanceDescription
            {
                ServiceId = "1",
                ApplicationName = "TestApp",
                ApplicationTypeName = "TestType"
            };
            descriptionRepository.GetInstanceDescriptionByIdAsync(Arg.Is(1)).Returns(instanceDescription);
            var deploymentDescription = new DeploymentDescription
            {
                DeploymentType = "remove", ApplicationInstanceId = "1"
            };
            var eventArgs = new BasicDeliverEventArgs
            {
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deploymentDescription))
            };
            var consumer = Substitute.For<IRmqConsumer>();
            consumer
                .When(w => w.StartListening(
                    Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                .Do((args) =>
                {
                    args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, eventArgs);
                });
            
            var deploymentStrategy = Substitute.For<IDeploymentStrategy>();
            deploymentStrategy.CanExecute(Arg.Is("deploy")).Returns(true);
            var removingStrategy = Substitute.For<IDeploymentStrategy>();
            removingStrategy.CanExecute(Arg.Is("remove")).Returns(true);
            var strategies = new List<IDeploymentStrategy> { deploymentStrategy, removingStrategy };
            
            var sut = new DeploymentService(consumer, descriptionRepository, strategies);
            
            // Act
            
            sut.Start();
            
            // Assert
            removingStrategy.Received(1).CanExecute(Arg.Is("remove"));
            removingStrategy.Received(1)
                .ProcessDeployment(
                    Arg.Is<InstanceDescription>(inDesc => 
                        inDesc.ApplicationName == instanceDescription.ApplicationName &&
                        inDesc.ApplicationTypeName == instanceDescription.ApplicationTypeName));
            consumer.Received(1).Ack(eventArgs);
        }

        [Test]
        public void ShouldSendAckAfterDeployment()
        {
            // Arrange
            var descriptionRepository = Substitute.For<IInstanceDescriptionRepository>();
            var instanceDescription = new InstanceDescription
            {
                ServiceId = "1",
                ApplicationName = "TestApp",
                ApplicationTypeName = "TestType"
            };
            descriptionRepository.GetInstanceDescriptionByIdAsync(Arg.Is(1)).Returns(instanceDescription);
            var deploymentDescription = new DeploymentDescription
            {
                DeploymentType = "deploy", ApplicationInstanceId = "1"
            };
            var eventArgs = new BasicDeliverEventArgs
            {
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deploymentDescription))
            };
            var consumer = Substitute.For<IRmqConsumer>();
            consumer
                .When(w => w.StartListening(
                    Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                .Do((args) =>
                {
                    args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, eventArgs);
                });
            
            var deploymentStrategy = Substitute.For<IDeploymentStrategy>();
            deploymentStrategy.CanExecute(Arg.Is("deploy")).Returns(true);
            var removingStrategy = Substitute.For<IDeploymentStrategy>();
            removingStrategy.CanExecute(Arg.Is("remove")).Returns(true);
            var strategies = new List<IDeploymentStrategy> { deploymentStrategy, removingStrategy };
            
            var sut = new DeploymentService(consumer, descriptionRepository, strategies);
            
            // Act
            
            sut.Start();
            
            // Assert
            Received.InOrder(() =>
            {
                deploymentStrategy.CanExecute(Arg.Is("deploy"));
                deploymentStrategy.ProcessDeployment(
                        Arg.Is<InstanceDescription>(inDesc => 
                            inDesc.ApplicationName == instanceDescription.ApplicationName &&
                            inDesc.ApplicationTypeName == instanceDescription.ApplicationTypeName));
                consumer.Ack(eventArgs);
            });
        }
    }
}