using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EDSc.Common.Model;
using EDSc.Common.Utils;

namespace EDSc.Common.Services
{
    public class RedditImgDownloadingService : IImgDownloadingService
    {
        private readonly IDataRetriever dataRetriever;
        private readonly string url = "https://old.reddit.com/r/aww/";
        private readonly Regex imageLinkRegex = new Regex(@"https:\/\/((i\.imgur\.com\/\S+\.jpg)|(i\.redd\.it\/\S+\.jpg))", RegexOptions.Compiled);
        private readonly Regex imgIdRegex = new Regex(@"\S+\/(\S+)\.jpg", RegexOptions.Compiled);

        public RedditImgDownloadingService(IDataRetriever dataRetriever)
        {
            this.dataRetriever = dataRetriever;
        }
        public async Task<IEnumerable<string>> GetImageLinksFromSource()
        {
            var sourcePage = await this.dataRetriever.GetStringAsync(url);
            return imageLinkRegex.Matches(sourcePage).Select(m => m.Value).Distinct();
        }
        public async Task<InMemoryImage> DownloadImage(string imgLink)
        {
            var img = new InMemoryImage
            {
                Id = this.ExtractImgId(imgLink),
                Url = imgLink,
                Image = await this.dataRetriever.GetByteArrayAsync(imgLink),
                DownloadingDate = DateTime.Now
            };
            return img;
        }

        private string ExtractImgId(string url)
        {
            return this.imgIdRegex.Match(url).Groups[1].Value;
        }
    }
}
