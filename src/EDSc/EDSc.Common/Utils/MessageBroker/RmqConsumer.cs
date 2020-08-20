namespace EDSc.Common.Utils.MessageBroker
{
    using System;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    
    public class RmqConsumer : IRmqConsumer
    {
        private EventingBasicConsumer BasicConsumer { get; }
        private readonly IModel amqpChannel;
        private string sourceQueue;

        public RmqConsumer(IModel amqpChannel, string queueName)
        {
            this.amqpChannel = amqpChannel;
            if (this.amqpChannel == null)
            {
                throw new ArgumentException("Amqp channel for Rmq consumer is null!");
            }
            this.BasicConsumer = new EventingBasicConsumer(this.amqpChannel);
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
            BasicConsumer.Received += onReceiveHandler;
            this.amqpChannel.BasicConsume(
                queue: this.sourceQueue,
                autoAck: false,
                consumer: this.BasicConsumer);
        }

        public void StopListening()
        {
            this.amqpChannel.BasicCancel(this.BasicConsumer.ConsumerTag);
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
