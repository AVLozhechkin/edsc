using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client.Events;

namespace EDSc.Common.MessageBroker
{
    public interface IRmqConsumer : IDisposable
    {
        void Ack(BasicDeliverEventArgs args);
        public void StartListening(EventHandler<BasicDeliverEventArgs> onReceiveHandler);
        public void StopListening();
        public void SetQueue(string queueName);
    }
}
