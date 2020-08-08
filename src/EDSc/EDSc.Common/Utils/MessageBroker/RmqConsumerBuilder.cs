namespace EDSc.Common.Utils.MessageBroker
{
    using System;
    using Microsoft.Extensions.Configuration;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    
    public class RmqConsumerBuilder : RmqBuilder<IRmqConsumer>
    {
        private string queueName = "Queue";
        private bool queueAutoCreate = false;

        public RmqConsumerBuilder UsingQueue(string queueName)
        {
            this.queueName = queueName;
            return this;
        }

        public RmqConsumerBuilder UsingConfigQueueName(IConfigurationSection configurationSection)
        {
            this.queueName = configurationSection.GetSection("Queue").Value;
            return this;
        }

        public RmqConsumerBuilder WithQueueAutoCreation()
        {
            this.queueAutoCreate = true;
            return this;
        }

        public override IRmqConsumer Build()
        {
            this.PrepareConsumerConnection();
            return new RmqConsumer(this.amqpChannel, this.queueName);
        }

        private void PrepareConsumerConnection()
        {
            connectionFactory = new ConnectionFactory
            {
                UserName = this.userName,
                Password = this.password,
                HostName = this.hostName,
                Port = this.port,
                VirtualHost = this.virtualHost
            };

            try
            {
                connection = connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException)
            {
                throw new NullReferenceException("Connection to rabbitmq server failed.");
            }
            this.amqpChannel = this.connection.CreateModel();

            if (this.queueAutoCreate)
            {
                this.amqpChannel.QueueDeclare(this.queueName);
            }
        }
    }
}
