namespace EDSc.Common.Services.Deployment.Strategies
{
    using EDSc.Common.Services.Deployment.Util;
    using System.Threading.Tasks;
    using EDSc.Common.Services.Deployment.Model;
    
    public class RemovingStrategy : IDeploymentStrategy
    {
        private const string StrategyName = "remove"; 
        private readonly ServiceFabricManager serviceFabricManager;

        public RemovingStrategy(ServiceFabricManager serviceFabricManager)
        {
            this.serviceFabricManager = serviceFabricManager;
        }

        public bool CanExecute(string deploymentType)
        {
            return deploymentType.Trim().ToLower() == StrategyName;
        }

        public async Task ProcessDeployment(InstanceDescription instanceDescription)
        {
            if (await this.serviceFabricManager.IsApplicationInstanceExistAsync(instanceDescription))
            {
                await this.serviceFabricManager.RemoveApplicationInstance(instanceDescription.ApplicationName);
            }

            if (await this.serviceFabricManager.IsApplicationTypeExistAsync(instanceDescription))
            {
                await this.serviceFabricManager.RemoveApplicationType(
                    instanceDescription.ApplicationTypeName,
                    instanceDescription.BuildVersion);
            }
        }
    }
}
