using System;

namespace EDSc.Common.Model
{
    public class InMemoryImageData
    {
        public byte[] Image { get; set; }
        public string ImgId { get; set; }
        public string ImgUrl { get; set; }
        public DateTime DownloadingDate { get; set; }
    }
}
