using EDSc.Common.Services.Deployment.Model;

namespace EDSc.Common.Services.Deployment.Database
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Bson;
    using MongoDB.Driver;
    
    public class MongoInstanceDescriptionRepository : IInstanceDescriptionRepository
    {
        private MongoClient Client { get; }
        private string DbName { get; }
        private string CollectionName { get; }
        public MongoInstanceDescriptionRepository(IConfigurationSection configurationSection, MongoClient client)
        {
            this.Client = client;
            this.DbName = configurationSection.GetValue<string>("Database");
            this.CollectionName = configurationSection.GetValue<string>("Collection");
        }
        public async Task<InstanceDescription> GetInstanceDescriptionByIdAsync(int id)
        {
            var descCol = this.Client.GetDatabase(this.DbName)
                .GetCollection<InstanceDescription>(this.CollectionName);
            var filter = new BsonDocument("ServiceId", id.ToString());
            var description = await descCol.Find(filter).ToListAsync();
            return description.Single();
        }
    }
}
