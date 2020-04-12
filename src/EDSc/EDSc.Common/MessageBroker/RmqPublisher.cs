using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace EDSc.Common.MessageBroker
{
    public class RmqPublisher : IRmqPublisher
    {
        private readonly IBasicProperties basicProperties;
        private readonly IModel amqpChannel;
        private readonly string exchange;
        private readonly string routingKey;

        public RmqPublisher(IModel amqpChannel, string exchange, string routingKey)
        {
            this.amqpChannel = amqpChannel;
            if (this.amqpChannel == null)
            {
                throw new ArgumentException("Amqp channel for Rmq publisher is null!");
            }
            this.basicProperties = this.amqpChannel.CreateBasicProperties();
            this.exchange = exchange;
            this.routingKey = routingKey;
        }

        ~RmqPublisher()
        {
            this.Dispose();
        }

        public void Publish(byte[] message)
        {
            this.amqpChannel.BasicPublish(
                exchange: this.exchange,
                routingKey: this.routingKey,
                basicProperties: this.basicProperties,
                body: message);

            this.amqpChannel.WaitForConfirms(TimeSpan.FromSeconds(1));
        }

        public void Publish(byte[] message, string routingKey)
        {
            this.amqpChannel.BasicPublish(
                exchange: this.exchange,
                routingKey: routingKey,
                basicProperties: this.basicProperties,
                body: message);

            this.amqpChannel.WaitForConfirms(TimeSpan.FromSeconds(1));
        }

        public void Publish(byte[] message, string exchange, string routingKey)
        {
            this.amqpChannel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: this.basicProperties,
                body: message);

            this.amqpChannel.WaitForConfirms(TimeSpan.FromSeconds(1));
        }

        public void Publish(byte[] outputData, string exchangeName, string[] routingKeys)
        {
            foreach (var rk in routingKeys)
            {
                this.Publish(outputData, exchangeName, rk);
            }
        }

        public void Dispose()
        {
            this.amqpChannel?.Close();
            this.amqpChannel?.Dispose();
        }
    }
}
