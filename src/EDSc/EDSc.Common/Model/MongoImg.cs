using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EDSc.Common.Model
{
    public class MongoImg
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public DateTime DownloadingDate { get; set; }
        public ObjectId ImgObjId { get; set; }
    }
}
