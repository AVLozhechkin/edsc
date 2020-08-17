namespace EDSc.Common.Services.Saving
{
    using System;
    using EDSc.Common.Services.Saving.Utils;
    using EDSc.Common.Utils.MessageBroker;
    using System.Text;
    using Dto;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;
    
    public class ImageSavingService
    {
        private IRmqConsumer Consumer { get; }
        private IImageToDbWriter<ImageDto> DbWriter { get; }

        public ImageSavingService(IImageToDbWriter<ImageDto> dbWriter, IRmqConsumer consumer)
        {
            this.DbWriter = dbWriter;
            this.Consumer = consumer;
        }
        public void Start()
        {
            Consumer.StartListening(ReceiveAndSaveImage);
        }

        private void ReceiveAndSaveImage(object sender, BasicDeliverEventArgs e)
        {
            var image = JsonConvert.DeserializeObject<ImageDto>(Encoding.UTF8.GetString(e.Body));

            if (image is null)
            {
                throw new ArgumentNullException();
            }

            this.DbWriter.SaveToDb(image);

            this.Consumer.Ack(e);
        }
    }
}
