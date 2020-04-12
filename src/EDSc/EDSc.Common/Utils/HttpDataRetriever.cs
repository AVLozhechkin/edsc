using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EDSc.Common.Utils
{
    public class HttpDataRetriever : IDisposable, IDataRetriever
    {
        private HttpClient httpClient = null;
        public HttpDataRetriever()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<byte[]> GetByteArrayAsync(string uri)
        {
            return await this.httpClient.GetByteArrayAsync(uri);
        }

        public async Task<string> GetStringAsync(string uri)
        {
            return await this.httpClient.GetStringAsync(uri);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
