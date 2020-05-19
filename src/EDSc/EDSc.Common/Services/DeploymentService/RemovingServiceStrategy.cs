namespace EDSc.Common.Services.Deployment
{
    using System.Threading.Tasks;
    using EDSc.Common.Services.Deployment.Model;

    public class RemovingServiceStrategy : IDeploymentStrategy
    {
        private readonly ServiceFabricManager serviceFabricManager;

        public RemovingServiceStrategy(ServiceFabricManager serviceFabricManager)
        {
            this.serviceFabricManager = serviceFabricManager;
        }

        public bool CanExecute(string deploymentType)
        {
            return deploymentType.Trim().ToLower() == "remove";
        }

        public async Task ProcessDeployment(ServiceDescription serviceDescription)
        {
            if (await this.serviceFabricManager.CheckIfApplicationInstanceExistAsync(
                serviceDescription.ApplicationName,
                serviceDescription.ApplicationTypeName,
                serviceDescription.BuildVersion))
            {
                await this.serviceFabricManager.RemoveApplicationInstance(serviceDescription.ApplicationName);
            }

            if (await this.serviceFabricManager.CheckIfApplicationTypeExistAsync(
                serviceDescription.ApplicationTypeName,
                serviceDescription.BuildVersion))
            {
                await this.serviceFabricManager.RemoveApplicationType(
                    serviceDescription.ApplicationTypeName,
                    serviceDescription.BuildVersion);
            }
        }
    }
}
