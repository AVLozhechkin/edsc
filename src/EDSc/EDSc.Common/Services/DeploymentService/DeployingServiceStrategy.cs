namespace EDSc.Common.Services.Deployment
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using EDSc.Common.Services.Deployment.Model;

    public class DeployingServiceStrategy : IDeploymentStrategy
    {
        private readonly IPackageManager packageManager;
        private readonly IConfigConverter<string, Dictionary<string, string>> configConverter;
        private readonly ServiceFabricManager serviceFabricManager;
        private readonly IServiceDescriptionRepository serviceDescriptionRepository;

        public DeployingServiceStrategy(
            IPackageManager packageManager, 
            IConfigConverter<string, Dictionary<string, string>> configConverter, 
            ServiceFabricManager serviceFabricManager,
            IServiceDescriptionRepository serviceDescriptionRepository)
        {
            this.packageManager = packageManager;
            this.configConverter = configConverter;
            this.serviceFabricManager = serviceFabricManager;
            this.serviceDescriptionRepository = serviceDescriptionRepository;
        }

        public bool CanExecute(string deploymentType)
        {
            return deploymentType.Trim().ToLower() == "deploy";
        }

        public async Task ProcessDeployment(ServiceDescription serviceDescription)
        {
            this.serviceFabricManager.InitializeConnection();

            if (await this.serviceFabricManager.CheckIfApplicationTypeExistAsync(
                serviceDescription.ApplicationTypeName, 
                serviceDescription.ApplicationTypeVersion))
            {
                if (await this.serviceFabricManager.CheckIfApplicationInstanceExistAsync(
                    serviceDescription.ApplicationName,
                    serviceDescription.ApplicationTypeName, 
                    serviceDescription.ApplicationTypeVersion))
                {
                    await this.serviceFabricManager.RemoveApplicationInstance(serviceDescription.ApplicationName);

                    var config = CreateConfig(serviceDescription);

                    await this.serviceFabricManager.CreateNewApplicationInstance(
                        serviceDescription.ApplicationName, 
                        serviceDescription.ApplicationTypeName,
                        serviceDescription.ApplicationTypeVersion,
                        config);
                }
                else
                {
                    var config = CreateConfig(serviceDescription);

                    await this.serviceFabricManager.CreateNewApplicationInstance(
                        serviceDescription.ApplicationName,
                        serviceDescription.ApplicationTypeName,
                        serviceDescription.ApplicationTypeVersion,
                        config);
                }
            }
            else
            {
                this.packageManager.ProvideTempPackageFolder(serviceDescription);
                this.packageManager.ArrangePackageStructure();

                var pathInImageStore = Guid.NewGuid().ToString();
                
                await this.serviceFabricManager.UploadApplicationPackage(this.packageManager.PathToCurrentTempFolder, pathInImageStore, true);
                this.packageManager.DeleteCurrentTempFolder();
                await this.serviceFabricManager.CreateApplicationType(pathInImageStore);

                // delete package from Image Store

                var config = CreateConfig(serviceDescription);

                await this.serviceFabricManager.CreateNewApplicationInstance(
                    serviceDescription.ApplicationName,
                    serviceDescription.ApplicationTypeName,
                    serviceDescription.ApplicationTypeVersion,
                    config);
            }
        }

        private Dictionary<string, string> CreateConfig(ServiceDescription serviceDescription)
        {
            return this.configConverter.Convert(serviceDescription.ConfigJson.ToString());
        }
    }
}
