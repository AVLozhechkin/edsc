using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EDSc.Common.MessageBroker
{
    public class RmqConsumer : IRmqConsumer
    {
        public EventingBasicConsumer basicConsumer { get; }
        private readonly IModel amqpChannel;
        private string sourceQueue;

        public RmqConsumer(IModel amqpChannel, string queueName)
        {
            this.amqpChannel = amqpChannel;
            if (this.amqpChannel == null)
            {
                throw new ArgumentException("Amqp channel for Rmq consumer is null!");
            }
            this.basicConsumer = new EventingBasicConsumer(this.amqpChannel);
            this.sourceQueue = queueName;
        }
        ~RmqConsumer()
        {
            this.Dispose();
        }
        public void Ack(BasicDeliverEventArgs args)
        {
            this.amqpChannel.BasicAck(args.DeliveryTag, false);
        }
        public void StartListening(EventHandler<BasicDeliverEventArgs> onReceiveHandler)
        {
            basicConsumer.Received += onReceiveHandler;
            this.amqpChannel.BasicConsume(
                queue: this.sourceQueue,
                autoAck: false,
                consumer: this.basicConsumer);
        }

        public void StopListening()
        {
            this.amqpChannel.BasicCancel(this.basicConsumer.ConsumerTag);
        }

        public void Dispose()
        {
            this.amqpChannel?.Close();
            this.amqpChannel?.Dispose();
        }

        public void SetQueue(string queueName)
        {
            sourceQueue = queueName;
        }
    }
}
