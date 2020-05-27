using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EDSc.Common.Services.Deployment.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace EDSc.Common.Services.Deployment
{
    public class MongoServiceDescriptionRepository : IServiceDescriptionRepository
    {
        private MongoClient Client { get; set; }
        private string DbName { get; set; }
        private string CollectionName { get; set; }
        public MongoServiceDescriptionRepository(IConfigurationSection configurationSection, MongoClient client)
        {
            this.Client = client;
            this.DbName = configurationSection.GetValue<string>("Database");
            this.CollectionName = configurationSection.GetValue<string>("Collection");
        }
        public async Task<ServiceDescription> GetServiceDescriptionByComponentIdAsync(int id)
        {
            var descCol = this.Client.GetDatabase(this.DbName).GetCollection<ServiceDescription>(this.CollectionName);
            var filter = new BsonDocument("ServiceId", id.ToString());
            var description = await descCol.Find(filter).ToListAsync();
            return description.Single();
        }
    }
}
