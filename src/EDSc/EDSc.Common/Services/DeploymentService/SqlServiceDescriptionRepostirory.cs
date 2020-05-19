namespace EDSc.Common.Services.Deployment
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using EDSc.Common.Services.Deployment.Model;
    using Microsoft.Extensions.Configuration;

    public class SqlServiceDescriptionRepostirory : IServiceDescriptionRepository
    {
        private const string ServiceDescriptionByIdQuery = @"SELECT [component].[ApplicationInstance].[InstanceName], [component].[ApplicationInstance].[ConfigJson], 
	                                        [meta].[ApplicationType].[TypeName], [meta].[ApplicationType].[TypeVersion]
                                            FROM [component].[ApplicationInstance]
                                            JOIN [meta].[ApplicationType] ON [meta].[ApplicationType].[TypeID] = [component].[ApplicationInstance].[TypeID]
                                            and [component].[ApplicationInstance].[InstanceID] = @ComponentId";
        private const string QueueNameByReceiverIdQuery = @"SELECT DISTINCT [component].[ApplicationInstance].[InstanceName]
                                            FROM [binding].[Binding]
                                            JOIN [component].[ApplicationInstance] ON [binding].[Binding].[ConsumerInstanceID] = @ComponentId 
                                            and [component].[ApplicationInstance].[InstanceId] = [binding].[Binding].[ConsumerInstanceID]";
        private const string ExchangeNameBySenderIdQuery = @"SELECT DISTINCT [component].[ApplicationInstance].[InstanceName]
                                            FROM [binding].[Binding]
                                            JOIN [component].[ApplicationInstance] ON [binding].[Binding].[PublisherInstanceID] = @ComponentId 
                                            and [component].[ApplicationInstance].[InstanceId] = [binding].[Binding].[PublisherInstanceID]";
        private const string RoutingKeysBySenderIdQuery = @"SELECT [component].[ApplicationInstance].[InstanceName]
                                            FROM [binding].[Binding]
                                            JOIN[component].[ApplicationInstance] ON[binding].[Binding].[PublisherInstanceID] = @ComponentId 
                                            and [component].[ApplicationInstance].[InstanceID] = [binding].[Binding].[ConsumerInstanceID]";

        private const string ConnectionSectionName = "ConnectionString";

        private readonly IConfigurationSection configurationSection;

        public SqlServiceDescriptionRepostirory(IConfigurationSection configurationSection)
        {
            this.configurationSection = configurationSection;
        }

        public async Task<string> GetExchangeNameBySenderIdAsync(int senderComponentId)
        {
            string connectionString = configurationSection.GetConnectionString(ConnectionSectionName);

            string exchangeName = null;

            using var sqlConnection = new SqlConnection(connectionString);
            using var sqlCommand = new SqlCommand(ExchangeNameBySenderIdQuery, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@ComponentId", senderComponentId);
            sqlConnection.Open();
            using var reader = sqlCommand.ExecuteReader();

            if (await reader.ReadAsync())
            {
                exchangeName = reader["InstanceName"].ToString();
            }

            return exchangeName;
        }

        public async Task<string> GetQueueNameByReceiverIdAsync(int receiverId)
        {
            string connectionString = configurationSection.GetConnectionString(ConnectionSectionName);

            string queueName = null;

            using var sqlConnection = new SqlConnection(connectionString);
            using var sqlCommand = new SqlCommand(QueueNameByReceiverIdQuery, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@ComponentId", receiverId);
            sqlConnection.Open();
            using var reader = sqlCommand.ExecuteReader();

            if (await reader.ReadAsync())
            {
                queueName = reader["InstanceName"].ToString();
            }

            return queueName;
        }

        public async Task<List<string>> GetReceivingComponentsBySenderIdAsync(int senderId)
        {
            string connectionString = configurationSection.GetConnectionString(ConnectionSectionName);

            var receivers = new List<string>();

            using var sqlConnection = new SqlConnection(connectionString);
            using var sqlCommand = new SqlCommand(RoutingKeysBySenderIdQuery, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@ComponentId", senderId);
            sqlConnection.Open();
            using var reader = sqlCommand.ExecuteReader();

            while (await reader.ReadAsync())
            {
                receivers.Add(reader["InstanceName"].ToString());
            }

            return receivers;
        }

        public async Task<ServiceDescription> GetServiceDescriptionByComponentIdAsync(int componentId)
        {
            string connectionString = configurationSection.GetConnectionString(ConnectionSectionName);

            var serviceDescription = new ServiceDescription();
            serviceDescription.ServiceId = componentId.ToString();
            using var sqlConnection = new SqlConnection(connectionString);
            using var sqlCommand = new SqlCommand(ServiceDescriptionByIdQuery, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@ComponentId", componentId);
            sqlConnection.Open();
            using var reader = sqlCommand.ExecuteReader();

            if (await reader.ReadAsync())
            {
                serviceDescription.ApplicationName = reader["InstanceName"].ToString();
                serviceDescription.ApplicationTypeName = reader["TypeName"].ToString();
                //serviceDescription.ConfigJson = reader["ConfigJson"].ToString();
                serviceDescription.BuildVersion = reader["TypeVersion"].ToString();
            }

            return serviceDescription;
        }
    }
}
