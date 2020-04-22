using System;

namespace EDSc.Common.Model
{
    public class Image
    {
        public byte[] ImgData { get; set; }
        public string ImgId { get; set; }
        public string ImgUrl { get; set; }
        public DateTime DownloadingDate { get; set; }
    }
}
