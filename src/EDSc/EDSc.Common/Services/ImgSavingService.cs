using EDSc.Common.MessageBroker;
using EDSc.Common.Model;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using EDSc.Common.Utils;
using System.Threading.Tasks;
using System.Runtime.ConstrainedExecution;

namespace EDSc.Common.Services
{
    public class ImgSavingService
    {
        private IRmqConsumer Consumer { get; }
        public IImgToDbWriter<InMemoryImage> DbWriter { get; }

        public ImgSavingService(IImgToDbWriter<InMemoryImage> dbWriter, IRmqConsumer consumer)
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
            var image = JsonConvert.DeserializeObject<InMemoryImage>(Encoding.UTF8.GetString(e.Body));

            var id = this.DbWriter.SaveToDb(image);

            this.Consumer.Ack(e);
        }
    }
}
