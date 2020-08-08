namespace EDSc.Common.Services.Saving.Model
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    
    public class MongoImage
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public string Label { get; set; }
        public float Score { get; set; }
        public DateTime DownloadingDate { get; set; }
        public ObjectId ImgObjId { get; set; }
    }
}
