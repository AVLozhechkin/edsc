namespace EDSc.Common.Services.Deployment.Model
{
    using MongoDB.Bson;
    
    public class InstanceDescription
    {
        public ObjectId Id { get; set; }
        public string ServiceId { get; set; }
        public string ApplicationName { get; set; }
        public string ConfigJson { get; set; }
        public string ApplicationTypeName { get; set; }
        public string BuildVersion { get; set; }
        public string ApplicationTypeVersion { get; set; } = "1.0.0";
    }
}
