namespace EDSc.Common.Services.Scraping
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Dto;
    using EDSc.Common.Services.Scraping.Utils;
    using Microsoft.Extensions.Configuration;
    
    public class RedditImageDownloadingService : IImageDownloadingService
    {
        private readonly IDataRetriever dataRetriever;
        private readonly string url = "https://old.reddit.com/r/";
        private readonly string subreddit;
        private readonly Regex imageLinkRegex = 
            new Regex(@"https:\/\/((i\.imgur\.com\/\S+\.jpg)|(i\.redd\.it\/\S+\.jpg))", 
                RegexOptions.Compiled);
        private readonly Regex imgIdRegex = new Regex(@"\S+\/(\S+)\.jpg", RegexOptions.Compiled);

        public RedditImageDownloadingService(IConfigurationSection section, IDataRetriever dataRetriever)
        {
            this.dataRetriever = dataRetriever;
            this.subreddit = section.GetSection("SubReddit").Value;
        }
        public async Task<IEnumerable<string>> GetImageLinksFromSource()
        {
            var sourcePage = await this.dataRetriever.GetStringAsync(Path.Combine(this.url, this.subreddit));
            return imageLinkRegex.Matches(sourcePage).Select(m => m.Value).Distinct();
        }
        public async Task<ImageDto> DownloadImage(string imgLink)
        {
            var img = new ImageDto
            {
                Id = this.ExtractImgId(imgLink),
                Url = imgLink,
                Image = await this.dataRetriever.GetByteArrayAsync(imgLink),
                DownloadingDate = DateTime.Now
            };
            return img;
        }

        private string ExtractImgId(string address)
        {
            return this.imgIdRegex.Match(address).Groups[1].Value;
        }
    }
}
