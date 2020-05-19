namespace EDSc.Common.Services.Deployment
{
    using EDSc.Common.Services.Deployment.Model;

    public interface IPackageManager
    {
        string PathToCurrentTempFolder { get; }
        void ProvideTempPackageFolder(ServiceDescription serviceDescription);
        void ArrangePackageStructure();
        void DeleteCurrentTempFolder();
    }
}
