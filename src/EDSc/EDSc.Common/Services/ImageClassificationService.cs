
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EDSc.Common.MessageBroker;
using EDSc.Common.Model;
using Microsoft.Extensions.ML;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace EDSc.Common.Services
{
    public class ImageClassificationService
    {
        private IRmqConsumer Consumer { get; }
        private IRmqPublisher Publisher { get; }
        private PredictionEnginePool<InMemoryImage, ImagePrediction> PredictionEnginePool { get; }

        public ImageClassificationService(
            IRmqConsumer consumer, 
            IRmqPublisher publisher, 
            PredictionEnginePool<InMemoryImage, ImagePrediction> predictionEnginePool)
        {
            Consumer = consumer;
            Publisher = publisher;
            PredictionEnginePool = predictionEnginePool;
        }

        public void Start()
        {
            Consumer.StartListening(ClassifyImage);
        }

        private void ClassifyImage(object sender, BasicDeliverEventArgs args)
        {
            var image = JsonConvert.DeserializeObject<InMemoryImage>(Encoding.UTF8.GetString(args.Body));
            var imagePrediction = this.PredictionEnginePool.Predict(image);

            foreach (var score in imagePrediction.Score)
            {
                if (score > 0.99)
                {
                    image.Label = imagePrediction.PredictedLabel;
                    image.Score = score;
                    this.Publisher.Publish(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(image)));
                }
            }
            Consumer.Ack(args);
        } 
    }
}