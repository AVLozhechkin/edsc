namespace EDSc.Common.Services.Classification
{
    using EDSc.Common.Utils.MessageBroker;
    using System.Text;
    using Dto;
    using EDSc.Common.Services.Classification.Model;
    using Microsoft.Extensions.ML;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;
    
    public class ImageClassificationService
    {
        private IRmqConsumer Consumer { get; }
        private IRmqPublisher Publisher { get; }
        private PredictionEnginePool<ImageDto, ImagePrediction> PredictionEnginePool { get; }

        public ImageClassificationService(
            IRmqConsumer consumer, 
            IRmqPublisher publisher, 
            PredictionEnginePool<ImageDto, ImagePrediction> predictionEnginePool)
        {
            this.Consumer = consumer;
            this.Publisher = publisher;
            this.PredictionEnginePool = predictionEnginePool;
        }

        public void Start()
        {
            this.Consumer.StartListening(ClassifyImage);
        }

        private void ClassifyImage(object sender, BasicDeliverEventArgs args)
        {
            var image = JsonConvert.DeserializeObject<ImageDto>(Encoding.UTF8.GetString(args.Body));
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