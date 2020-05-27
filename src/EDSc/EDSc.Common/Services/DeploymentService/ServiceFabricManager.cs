namespace EDSc.Common.Services.Deployment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Common;

    public class ServiceFabricManager 
    {
        private readonly string sfClusterUri;
        private IServiceFabricClient sfClient;

        public ServiceFabricManager(IConfigurationSection section)
        {
            this.sfClusterUri = section.GetValue<string>("sfClusterUri");
        }

        public void InitializeConnection()
        {
            this.sfClient = new ServiceFabricClientBuilder()
                    .UseEndpoints(new Uri(sfClusterUri))
                    .BuildAsync().Result;
        }

        public async Task<bool> CheckIfApplicationTypeExistAsync(string typeName, string typeVersion)
        {
            return (await this.sfClient.ApplicationTypes.GetApplicationTypeInfoListByNameAsync(typeName, typeVersion)).Data.Any();
        }

        public async Task<bool> CheckIfApplicationInstanceExistAsync(string appName, string typeName, string typeVersion)
        {
            return (await this.sfClient.Applications.GetApplicationInfoListAsync()).Data
                .Any(app => app.Name == "fabric:/" + appName && app.TypeName == typeName && app.TypeVersion == typeVersion);
        }

        public async Task CreateNewApplicationInstance(
            string applicationName, 
            string applicationTypeName, 
            string applicationTypeVersion, 
            Dictionary<string, string> appParams)
        {
            await this.sfClient.Applications.CreateApplicationAsync(
                new ApplicationDescription("fabric:/" + applicationName, applicationTypeName, applicationTypeVersion, appParams));
        }

        public async Task CreateApplicationType(string applicationPathInTheImageStore)
        {
            await sfClient.ApplicationTypes.ProvisionApplicationTypeAsync(new ProvisionApplicationTypeDescription(applicationPathInTheImageStore));
        }

        public async Task UploadApplicationPackage(string pathToTheLocalPackage, string applicationPathInTheImageStore, bool compressPackage)
        {
            await this.sfClient.ImageStore.UploadApplicationPackageAsync(pathToTheLocalPackage, compressPackage, applicationPathInTheImageStore);
        }

        public async Task RemoveApplicationInstance(string applicationName)
        {
            await this.sfClient.Applications.DeleteApplicationAsync(applicationName);
        }

        public async Task RemoveApplicationType(string applicationTypeName, string typeVersion)
        {
            await this.sfClient.ApplicationTypes.UnprovisionApplicationTypeAsync(
                applicationTypeName,
                new UnprovisionApplicationTypeDescriptionInfo(typeVersion));
        }

        public async Task RemoveApplicationPackage()
        {
            throw new NotImplementedException();
        }
    }
}
