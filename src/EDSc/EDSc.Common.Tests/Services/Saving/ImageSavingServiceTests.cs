using System;
using System.Drawing;
using System.Linq;
using System.Text;
using EDSc.Common.Dto;
using EDSc.Common.Services.Saving;
using EDSc.Common.Services.Saving.Model;
using EDSc.Common.Services.Saving.Utils;
using EDSc.Common.Utils.MessageBroker;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client.Events;

namespace EDSc.Common.Tests.Services.Saving
{
    [TestFixture]
    public class ImageSavingServiceTests
    {
        [Test]
        public void ShouldSaveAndAcknowledge()
        {
            // Arrange
            var imageToDbWriter = Substitute.For<IImageToDbWriter<ImageDto>>();
            var consumer = Substitute.For<IRmqConsumer>();
            var imageDto = new ImageDto { Id = "abcde" };
            var serialisedImageDto = JsonConvert.SerializeObject(imageDto);
            var eventArgs = new BasicDeliverEventArgs()
            {
                Body = Encoding.UTF8.GetBytes(serialisedImageDto)
            };
            consumer
                .When(w => w.StartListening(
                    Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                .Do((args) =>
                {
                    args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, eventArgs);
                });
            var sut = new ImageSavingService(imageToDbWriter, consumer);
            
            // Act
            sut.Start();

            // Assert
            Received.InOrder(() =>
            {
                imageToDbWriter.SaveToDb(Arg.Is<ImageDto>(img => img.Id == imageDto.Id));
                consumer.Ack(Arg.Is<BasicDeliverEventArgs>(
                    ea => ea.Body.SequenceEqual(eventArgs.Body)));
            });
            imageToDbWriter.Received(1).SaveToDb(Arg.Is<ImageDto>(img => img.Id == imageDto.Id));
            consumer.Received(1).Ack(Arg.Is<BasicDeliverEventArgs>(
                ea => ea.Body.SequenceEqual(eventArgs.Body)));
        }

        [Test]
        public void ShouldThrowExceptionIfImageIsNull()
        {
            // Arrange
            var imageToDbWriter = Substitute.For<IImageToDbWriter<ImageDto>>();
            var consumer = Substitute.For<IRmqConsumer>();
            ImageDto imageDto = null;
            var serialisedImageDto = JsonConvert.SerializeObject(imageDto);
            var eventArgs = new BasicDeliverEventArgs()
            {
                Body = Encoding.UTF8.GetBytes(serialisedImageDto)
            };
            consumer
                .When(w => w.StartListening(
                    Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                .Do((args) =>
                {
                    args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, eventArgs);
                });
            var sut = new ImageSavingService(imageToDbWriter, consumer);
            
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => sut.Start());
        }
    }
}