namespace EDSc.Common.Services.Deployment.Util
{
    using EDSc.Common.Services.Deployment.Model;
    
    public interface IPackageManager
    {
        string PathToCurrentTempFolder { get; }
        void ProvideTempPackageFolder(InstanceDescription instanceDescription);
        void ArrangePackageStructure();
        void DeleteCurrentTempFolder();
    }
}
