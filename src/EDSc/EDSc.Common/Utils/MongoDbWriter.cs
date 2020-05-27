using EDSc.Common.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tensorflow.Keras;

namespace EDSc.Common.Utils
{
    public class ImgToMongoWriter : IImgToDbWriter<InMemoryImage>, IDisposable
    {
        private MongoClient Client { get; set; }
        private string DbName { get; set; }
        private string CollectionName { get; set; }
        public ImgToMongoWriter(IConfigurationSection configurationSection, MongoClient client)
        {
            this.Client = client;
            this.DbName = configurationSection.GetValue<string>("Database");
            this.CollectionName = configurationSection.GetValue<string>("Collection");
        }
        public string SaveToDb(InMemoryImage img)
        {
            var collection = this.Client.GetDatabase(this.DbName).GetCollection<MongoImg>(this.CollectionName);

            var mongoImg = new MongoImg
            {
                Id = img.Id,
                Url = img.Url,
                Score = img.Score,
                Label = img.Label,
                DownloadingDate = img.DownloadingDate,
                ImgObjId = UploadImg(img.Id, img.Image)
            };

            collection.InsertOne(mongoImg);
            
            return mongoImg.ObjectId.ToString();
        }
        private ObjectId UploadImg(string name, byte[] img)
        {
            var db = this.Client.GetDatabase(this.DbName);
            IGridFSBucket gridFS = new GridFSBucket(db);
            return gridFS.UploadFromBytes(name, img);

        }

        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

        }
    }
}
