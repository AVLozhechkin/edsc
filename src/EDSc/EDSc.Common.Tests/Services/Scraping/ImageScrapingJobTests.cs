namespace EDSc.Common.Tests.Services.Scraping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EDSc.Common.Dto;
    using EDSc.Common.Services.Scraping;
    using EDSc.Common.Utils.MessageBroker;
    using Newtonsoft.Json;
    using NSubstitute;
    using NUnit.Framework;
    using Quartz;
    
    [TestFixture]
    public class ImageScrapingJobTests
    {
        [Test]
        public void ShouldPublishAllImages()
        {
            // Arrange
            var testImage1 = new ImageDto { Url = "https://i.imgur.com/Zt4ZU86.jpg", Id = "Zt4ZU86" };
            var testImage2 = new ImageDto { Url = "https://i.redd.it/2xin7kun7es41.jpg", Id = "2xin7kun7es41" };
            var imgDlService = Substitute.For<IImageDownloadingService>();
            var publisher = Substitute.For<IRmqPublisher>();
            var jobCtx = Substitute.For<IJobExecutionContext>();
            var jobDataMap = new JobDataMap()
            {
                {"IRmqPublisher", publisher},
                {"IImageDownloadingService", imgDlService}
            };
            jobCtx.JobDetail.JobDataMap.Returns(jobDataMap);

            imgDlService.GetImageLinksFromSource().Returns(
                new List<string>
                {
                    testImage1.Url, testImage2.Url
                });
            
            imgDlService.DownloadImage(Arg.Is(testImage1.Url)).Returns(testImage1);
            imgDlService.DownloadImage(Arg.Is(testImage2.Url)).Returns(testImage2);

            var encodedTestImage1 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testImage1));
            var encodedTestImage2 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testImage2));
            
            var sut = new ImageScrapingJob();
            
            // Act
            sut.Execute(jobCtx).Wait();
            
            // Assert
            imgDlService.Received(1).GetImageLinksFromSource();
            imgDlService.Received(1).DownloadImage(Arg.Is(testImage1.Url));
            imgDlService.Received(1).DownloadImage(Arg.Is(testImage2.Url));
            publisher.Received(1).Publish(Arg.Is<byte[]>(arg => arg.SequenceEqual(encodedTestImage1)));
            publisher.Received(1).Publish(Arg.Is<byte[]>(arg => arg.SequenceEqual(encodedTestImage2)));
        }
        [Test]
        public void ShouldThrowExceptionWhenNoPublisherProvided()
        {
            // Arrange
            var imgDlService = Substitute.For<IImageDownloadingService>();
            var jobCtx = Substitute.For<IJobExecutionContext>();
            var jobDataMap = new JobDataMap()
            {
                {"IImageDownloadingService", imgDlService}
            };
            jobCtx.JobDetail.JobDataMap.Returns(jobDataMap);
            
            var sut = new ImageScrapingJob();
            
            // Act/Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.Execute(jobCtx));
            Assert.IsTrue(string.Equals(exception.ParamName, "Publisher"));
        }
        [Test]
        public void ShouldThrowExceptionWhenNoImageDlServiceProvided()
        {
            // Arrange
            var publisher = Substitute.For<IRmqPublisher>();
            var jobCtx = Substitute.For<IJobExecutionContext>();
            var jobDataMap = new JobDataMap()
            {
                {"IRmqPublisher", publisher}
            };
            jobCtx.JobDetail.JobDataMap.Returns(jobDataMap);
            
            var sut = new ImageScrapingJob();

            // Act/Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.Execute(jobCtx));
            Assert.IsTrue(string.Equals(exception.ParamName, "ImageDownloadingService"));
        }
    }
}