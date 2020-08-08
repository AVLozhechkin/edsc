using EDSc.Common.Services.Deployment.Util;

namespace EDSc.Common.Services.Deployment.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EDSc.Common.Services.Deployment.Model;
    
    public class DeployingStrategy : IDeploymentStrategy
    {
        private const string StrategyName = "deploy";
        private IPackageManager PackageManager { get; }
        private IConfigConverter<string, Dictionary<string, string>> ConfigConverter { get; }
        private ServiceFabricManager SfManager { get; }

        public DeployingStrategy(
            IPackageManager packageManager, 
            IConfigConverter<string, Dictionary<string, string>> configConverter, 
            ServiceFabricManager sfManager)
        {
            this.PackageManager = packageManager;
            this.ConfigConverter = configConverter;
            this.SfManager = sfManager;
        }

        public bool CanExecute(string deploymentType)
        {
            return deploymentType.Trim().ToLower() == StrategyName;
        }

        public async Task ProcessDeployment(InstanceDescription instanceDescription)
        {
            this.SfManager.InitializeConnection();

            if (await this.SfManager.IsApplicationTypeExistAsync(instanceDescription))
            {
                if (await this.SfManager.IsApplicationInstanceExistAsync(instanceDescription))
                {
                    await this.SfManager.RemoveApplicationInstance(instanceDescription.ApplicationName);

                    var config = CreateConfigFromJson(instanceDescription.ConfigJson);

                    await this.SfManager.CreateNewApplicationInstance(instanceDescription, config);
                }
                else
                {
                    var config = CreateConfigFromJson(instanceDescription.ConfigJson);

                    await this.SfManager.CreateNewApplicationInstance(instanceDescription, config);
                }
            }
            else
            {
                this.PackageManager.ProvideTempPackageFolder(instanceDescription);
                this.PackageManager.ArrangePackageStructure();

                var pathInImageStore = Guid.NewGuid().ToString();
                
                await this.SfManager.UploadApplicationPackage(
                    this.PackageManager.PathToCurrentTempFolder, 
                    pathInImageStore, 
                    true);
                this.PackageManager.DeleteCurrentTempFolder();
                await this.SfManager.CreateApplicationType(pathInImageStore);

                // delete package from Image Store

                var config = CreateConfigFromJson(instanceDescription.ConfigJson);

                await this.SfManager.CreateNewApplicationInstance(instanceDescription, config);
            }
        }

        private Dictionary<string, string> CreateConfigFromJson(string json)
        {
            return this.ConfigConverter.Convert(json);
        }
    }
}
