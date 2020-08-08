namespace EDSc.Common.Utils.MessageBroker
{
    using System;
    using Microsoft.Extensions.Configuration;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    
    public class RmqPublisherBuilder : RmqBuilder<IRmqPublisher>
    {
        private string exchange = "default";
        private string routingKey = "default";
        private bool exchangeAutoCreate = false;

        public RmqPublisherBuilder UsingExchange(string exchange)
        {
            this.exchange = exchange;
            return this;
        }

        public RmqPublisherBuilder UsingRoutingKey(string routingKey)
        {
            this.routingKey = routingKey;
            return this;
        }

        public RmqPublisherBuilder UsingExchangeAndRoutingKey(string exchange, string routingKey)
        {
            this.exchange = exchange;
            this.routingKey = routingKey;
            return this;
        }

        public RmqPublisherBuilder UsingConfigExchangeAndRoutingKey(IConfigurationSection configurationSection)
        {
            this.exchange = configurationSection.GetSection("SenderExchange").Value;
            this.routingKey = configurationSection.GetSection("SenderRoutingKeys").Value;
            return this;
        }

        public RmqPublisherBuilder WithExchangeAutoCreation()
        {
            this.exchangeAutoCreate = true;
            return this;
        }

        public override IRmqPublisher Build()
        {
            this.PreparePublisherConnection();
            return new RmqPublisher(amqpChannel, exchange, routingKey);
        }

        private void PreparePublisherConnection()
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
            this.amqpChannel.ConfirmSelect();

            if (this.exchangeAutoCreate)
            {
                this.amqpChannel.ExchangeDeclare(this.exchange, "direct");
            }
        }
    }
}
