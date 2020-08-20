namespace EDSc.Common.Tests.Services.Scraping
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using EDSc.Common.Services.Scraping;
    using EDSc.Common.Services.Scraping.Utils;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using NUnit.Framework;
    
    [TestFixture]
    public class RedditImageDownloadingServiceTests
    {
        [Test]
        public void ShouldReturnEmptyListWhenNoLinksInSource()
        {
            // Arrange
            var dataRetriever = Substitute.For<IDataRetriever>();
            var url = "https://old.reddit.com/r/aww/";
            var subreddit = new Dictionary<string, string>
            {
                {"ImageDownloadingService:SubReddit", "aww"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(subreddit)
                .Build();
            dataRetriever.GetStringAsync(
                Arg.Is(url)).Returns(string.Empty);
            var sut = new RedditImageDownloadingService(
                configuration.GetSection("ImageDownloadingService"), 
                dataRetriever);
            
            // Act
            var links = sut.GetImageLinksFromSource().Result;
            
            // Assert
            Assert.IsFalse(links.Any());
        }
        [Test]
        public void ShouldReturnLinksListWhenSourceIsCorrect()
        {
            // Arrange
            var dataRetriever = Substitute.For<IDataRetriever>();
            var subreddit = "aww";
            var url = Path.Combine("https://old.reddit.com/r/", subreddit);
            var configData = new Dictionary<string, string>
            {
                {"ImageDownloadingService:SubReddit", subreddit}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
            
            dataRetriever.GetStringAsync(
                Arg.Is(url)).Returns(
                File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory,
                    "Services\\Scraping\\Source.html")));

            var linksInSource = new List<string>
            {
                "https://i.redd.it/v7nqb0sd0es41.jpg",
                "https://i.redd.it/9x8qrpbynds41.jpg",
                "https://i.redd.it/fxss3d12nes41.jpg",
                "https://i.redd.it/60ykozqysds41.jpg",
                "https://i.redd.it/y67o559upds41.jpg",
                "https://i.redd.it/p8vykcgi8es41.jpg",
                "https://i.redd.it/2xin7kun7es41.jpg",
                "https://i.imgur.com/Zt4ZU86.jpg",

            };
            var sut = new RedditImageDownloadingService(
                configuration.GetSection("ImageDownloadingService"), 
                dataRetriever);

            // Act
            var links = sut.GetImageLinksFromSource().Result;

            // Assert
            Assert.IsTrue(links.SequenceEqual(linksInSource));
        }
        [Test]
        public void ShouldDownloadImageFromUrl()
        {
            // Arrange
            var emptyConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var link = "https://i.redd.it/v7nqb0sd0es41.jpg";
            var imageId = "v7nqb0sd0es41";
            var dataRetriever = Substitute.For<IDataRetriever>();
            var testImgBytes = Encoding.UTF8.GetBytes("Test");
            dataRetriever.GetByteArrayAsync(
                Arg.Is(link))
                .Returns(testImgBytes);

            var sut = new RedditImageDownloadingService(
                emptyConfiguration.GetSection(string.Empty), 
                dataRetriever);

            // Act
            var downloadedImg = sut.DownloadImage(link).Result;

            // Assert
            Assert.IsTrue(downloadedImg.Url == link);
            Assert.IsTrue(downloadedImg.Image == testImgBytes);
            Assert.IsTrue(downloadedImg.Id == imageId);
            Assert.IsNotNull(downloadedImg.DownloadingDate);
        }
    }
}