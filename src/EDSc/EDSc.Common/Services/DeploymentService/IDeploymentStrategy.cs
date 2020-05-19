namespace EDSc.Common.Services.Deployment
{
    using System.Threading.Tasks;
    using EDSc.Common.Services.Deployment.Model;

    public interface IDeploymentStrategy
    {
        bool CanExecute(string deploymentType);
        Task ProcessDeployment(ServiceDescription serviceDescription);
    }
}
