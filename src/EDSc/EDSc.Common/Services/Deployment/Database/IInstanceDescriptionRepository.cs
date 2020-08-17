namespace EDSc.Common.Services.Deployment.Database
{
    using EDSc.Common.Services.Deployment.Model;
    using System.Threading.Tasks;
    
    public interface IInstanceDescriptionRepository
    {
        Task<InstanceDescription> GetInstanceDescriptionByIdAsync(int id);
    }
}
