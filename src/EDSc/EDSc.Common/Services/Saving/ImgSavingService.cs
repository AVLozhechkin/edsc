using EDSc.Common.Utils.MessageBroker;

namespace EDSc.Common.Services.Saving
{
    using System.Text;
    using Dto;
    using EDSc.Common.Utils;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;
    
    public class ImgSavingService
    {
        private IRmqConsumer Consumer { get; }
        private IImgToDbWriter<ImageDto> DbWriter { get; }

        public ImgSavingService(IImgToDbWriter<ImageDto> dbWriter, IRmqConsumer consumer)
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

            var id = this.DbWriter.SaveToDb(image);

            this.Consumer.Ack(e);
        }
    }
}
