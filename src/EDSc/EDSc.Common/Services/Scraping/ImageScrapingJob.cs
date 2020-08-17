using System;
using EDSc.Common.Utils.MessageBroker;

namespace EDSc.Common.Services.Scraping
{
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Quartz;
    
    public class ImageScrapingJob : IJob
    {
        private IImageDownloadingService ImageDownloadingService { get; set; }
        private IRmqPublisher Publisher { get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            
            this.Publisher = (IRmqPublisher)dataMap["IRmqPublisher"];
            if (this.Publisher is null)
            {
                throw new ArgumentNullException(nameof(this.Publisher));
            }
            
            this.ImageDownloadingService = (IImageDownloadingService)dataMap["IImageDownloadingService"];
            if (this.ImageDownloadingService is null)
            {
                throw new ArgumentNullException(nameof(this.ImageDownloadingService));
            }
            
            var imgLinks = await this.ImageDownloadingService.GetImageLinksFromSource();
            Parallel.ForEach(imgLinks, ProcessImage);


        }
        private async void ProcessImage(string url)
        {
            var img = await this.ImageDownloadingService.DownloadImage(url);

            var serializedImg = JsonConvert.SerializeObject(img);

            this.Publisher.Publish(Encoding.UTF8.GetBytes(serializedImg));
        }
    }
}
