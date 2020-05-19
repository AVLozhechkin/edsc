namespace EDSc.Common.Services.Deployment
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EDSc.Common.Services.Deployment.Model;

    public interface IServiceDescriptionRepository
    {
        Task<ServiceDescription> GetServiceDescriptionByComponentIdAsync(int id);
    }
}
