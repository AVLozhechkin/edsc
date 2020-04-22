using System.Collections.Generic;
using System.Threading.Tasks;
using EDSc.Common.Model;

namespace EDSc.Common.Services
{
    public interface IImgDownloadingService
    {
        Task<IEnumerable<string>> GetImageLinksFromSource();
        Task<Image> DownloadImage(string imgLink);
    }
}