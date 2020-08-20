namespace EDSc.Common.Services.Scraping.Utils
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    
    public class HttpDataRetriever : IDisposable, IDataRetriever
    {
        private HttpClient HttpClient { get; }
        public HttpDataRetriever()
        {
            this.HttpClient = new HttpClient();
        }

        public async Task<byte[]> GetByteArrayAsync(string uri)
        {
            return await this.HttpClient.GetByteArrayAsync(uri);
        }

        public async Task<string> GetStringAsync(string uri)
        {
            return await this.HttpClient.GetStringAsync(uri);
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}
