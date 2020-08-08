namespace EDSc.Common.Services.Deployment.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EDSc.Common.Services.Deployment.Model;
    using Microsoft.Extensions.Configuration;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Common;
    
    public class ServiceFabricManager : IDisposable
    {
        private string uriSectionName = "sfClusterUri";
        private readonly string sfClusterUri;
        private IServiceFabricClient SfClient { get; set; }

        public ServiceFabricManager(IConfigurationSection section)
        {
            this.sfClusterUri = section.GetValue<string>(uriSectionName);
        }

        public async void InitializeConnection()
        {
            this.SfClient = await new ServiceFabricClientBuilder()
                    .UseEndpoints(new Uri(sfClusterUri))
                    .BuildAsync();
        }

        public async Task<bool> IsApplicationTypeExistAsync(InstanceDescription description)
        {
            return (await this.SfClient.ApplicationTypes.GetApplicationTypeInfoListByNameAsync(
                description.ApplicationTypeName, 
                description.ApplicationTypeVersion)).Data.Any();
        }

        public async Task<bool> IsApplicationInstanceExistAsync(InstanceDescription description)
        {
            return (await this.SfClient.Applications.GetApplicationInfoListAsync()).Data
                .Any(app => app.Name == "fabric:/" + description.ApplicationName &&
                            app.TypeName == description.ApplicationTypeName &&
                            app.TypeVersion == description.ApplicationTypeVersion);
        }

        public async Task CreateNewApplicationInstance(InstanceDescription description, 
            Dictionary<string, string> appParams)
        {
            await this.SfClient.Applications.CreateApplicationAsync(
                new ApplicationDescription("fabric:/" + description.ApplicationName, 
                    description.ApplicationTypeName, 
                    description.ApplicationTypeVersion, 
                    appParams));
        }

        public async Task CreateApplicationType(string applicationPathInTheImageStore)
        {
            await SfClient.ApplicationTypes.ProvisionApplicationTypeAsync(
                new ProvisionApplicationTypeDescription(applicationPathInTheImageStore));
        }

        public async Task UploadApplicationPackage(
            string pathToTheLocalPackage, 
            string applicationPathInTheImageStore, 
            bool compressPackage)
        {
            await this.SfClient.ImageStore.UploadApplicationPackageAsync(
                pathToTheLocalPackage, compressPackage, applicationPathInTheImageStore);
        }

        public async Task RemoveApplicationInstance(string applicationName)
        {
            await this.SfClient.Applications.DeleteApplicationAsync(applicationName);
        }

        public async Task RemoveApplicationType(string applicationTypeName, string typeVersion)
        {
            await this.SfClient.ApplicationTypes.UnprovisionApplicationTypeAsync(
                applicationTypeName, 
                new UnprovisionApplicationTypeDescriptionInfo(typeVersion));
        }

        public Task RemoveApplicationPackage()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            SfClient?.Dispose();
        }
    }
}
