namespace EDSc.Common.Services.Scraping
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dto;
    
    public interface IImageDownloadingService
    {
        Task<IEnumerable<string>> GetImageLinksFromSource();
        Task<ImageDto> DownloadImage(string imgLink);
    }
}