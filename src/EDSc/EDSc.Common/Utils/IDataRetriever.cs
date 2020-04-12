using System.Threading.Tasks;

namespace EDSc.Common.Utils
{
    public interface IDataRetriever
    {
        Task<string> GetStringAsync(string uri);
        Task<byte[]> GetByteArrayAsync(string uri);
    }
}