using System;

namespace EDSc.Common.Model
{
    public class InMemoryImage
    {
        public byte[] Image { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public DateTime DownloadingDate { get; set; }
    }
}
