using EDSc.Common.Services.Deployment.Model;

namespace EDSc.Common.Services.Deployment.Database
{
    using System.Threading.Tasks;
    
    public interface IInstanceDescriptionRepository
    {
        Task<InstanceDescription> GetInstanceDescriptionByIdAsync(int id);
    }
}
