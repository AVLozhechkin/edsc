namespace EDSc.Common.Dto
{
    using System;
    
    public class ImageDto
    {
        public byte[] Image { get; set; }
        public string Label { get; set; }
        public float Score { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public DateTime DownloadingDate { get; set; }
    }
}
