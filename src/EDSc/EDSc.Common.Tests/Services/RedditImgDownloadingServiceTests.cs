using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EDSc.Common.Services;
using EDSc.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace EDSc.Common.Tests.Services
{
    [TestClass]
    public class RedditImgDownloadingServiceTests
    {
        [TestMethod]
        public void ShouldReturnLinksListWhenSourceIsCorrect()
        {
            // arrange
            var dataRetriever = Substitute.For<IDataRetriever>();
            var url = "https://old.reddit.com/r/aww/";
            dataRetriever.GetStringAsync(
                Arg.Is(url)).Returns(
                    File.ReadAllText("Services/Source.html"));
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
            var sut = new RedditImgDownloadingService(dataRetriever);

            // act
            var links = sut.GetImageLinksFromSource().Result;

            // assert
            Assert.IsTrue(links.SequenceEqual(linksInSource));
        }
        [TestMethod]
        public void ShouldDownloadImageFromGivenUrl()
        {
            // Arrange
            var link = "https://i.redd.it/v7nqb0sd0es41.jpg";
            var imageId = "v7nqb0sd0es41";
            var dataRetriever = Substitute.For<IDataRetriever>();
            var testImgBytes = Encoding.UTF8.GetBytes("Test");
            dataRetriever.GetByteArrayAsync(
                Arg.Is(link))
                .Returns(testImgBytes);

            var sut = new RedditImgDownloadingService(dataRetriever);

            // Act
            var downloadedImg = sut.DownloadImage(link).Result;

            // Assert
            Assert.IsTrue(downloadedImg.Url == link);
            Assert.IsTrue(downloadedImg.Image == testImgBytes);
            Assert.IsTrue(downloadedImg.Id == imageId);
        }
    }
}
