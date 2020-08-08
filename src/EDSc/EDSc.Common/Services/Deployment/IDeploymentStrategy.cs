using EDSc.Common.Services.Deployment.Model;

namespace EDSc.Common.Services.Deployment
{
    using System.Threading.Tasks;

    public interface IDeploymentStrategy
    {
        bool CanExecute(string deploymentType);
        Task ProcessDeployment(InstanceDescription instanceDescription);
    }
}
