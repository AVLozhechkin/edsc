using System.Text;
using System.Threading.Tasks;
using EDSc.Common.MessageBroker;
using Newtonsoft.Json;
using Quartz;

namespace EDSc.Common.Services
{
    public class ImgScrapingJob : IJob
    {
        private IImgDownloadingService imgDownloadingService;
        private IRmqPublisher rmqPublisher;

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            this.rmqPublisher = (IRmqPublisher)dataMap["IRmqPublisher"];
            this.imgDownloadingService = (IImgDownloadingService)dataMap["IImgDownloadingService"];
            var imgLinks = await this.imgDownloadingService.GetImageLinksFromSource();
            Parallel.ForEach(imgLinks, ProcessImage);


        }
        private async void ProcessImage(string url)
        {
            var img = await this.imgDownloadingService.DownloadImage(url);

            var serializedImg = JsonConvert.SerializeObject(img);

            this.rmqPublisher.Publish(Encoding.UTF8.GetBytes(serializedImg));
        }
    }
}
