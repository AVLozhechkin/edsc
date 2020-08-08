namespace EDSc.Common.Services.Scraping.Utils
{
    using System.Threading.Tasks;
    
    public interface IDataRetriever
    {
        Task<string> GetStringAsync(string uri);
        Task<byte[]> GetByteArrayAsync(string uri);
    }
}