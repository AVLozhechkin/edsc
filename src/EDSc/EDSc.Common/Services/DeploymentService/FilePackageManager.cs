namespace EDSc.Common.Services.Deployment
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using EDSc.Common.Services.Deployment.Model;
    using Microsoft.Extensions.Configuration;

    public class FilePackageManager : IPackageManager
    {
        private string pathToDropFolder;
        private string pathToBaseTempFolder;

        public FilePackageManager(IConfigurationSection configurationSection)
        {
            this.pathToDropFolder = configurationSection.GetValue<string>("pathToDropFolder");
            this.pathToBaseTempFolder = configurationSection.GetValue<string>("pathToTempFolder");
        }

        public string PathToCurrentTempFolder { get; private set; }

        public void ProvideTempPackageFolder(ServiceDescription serviceDescription)
        {
            this.PathToCurrentTempFolder = Path.Combine(this.pathToBaseTempFolder, Guid.NewGuid().ToString());
            ZipFile.ExtractToDirectory(
                Path.Combine(
                    this.pathToDropFolder, serviceDescription.BuildVersion, 
                    serviceDescription.ApplicationTypeName + ".zip"),
                Path.Combine(this.PathToCurrentTempFolder, "Temp"));
        }

        public void ArrangePackageStructure()
        {
            var servicePackagePath = Path.Combine(this.PathToCurrentTempFolder, "ServicePackage");
            var codePackagePath = Path.Combine(servicePackagePath, "Code");
            var configPackagePath = Path.Combine(servicePackagePath, "Config");
            var pathToTheManifests = Path.Combine(this.PathToCurrentTempFolder, "Temp", "ServiceFabric");

            Directory.CreateDirectory(servicePackagePath);
            Directory.CreateDirectory(configPackagePath);

            Directory.Move(
                Path.Combine(pathToTheManifests, "ApplicationManifest.xml"),
                Path.Combine(this.PathToCurrentTempFolder, "ApplicationManifest.xml"));
            Directory.Move(
                Path.Combine(pathToTheManifests, "ServiceManifest.xml"),
                Path.Combine(servicePackagePath, "ServiceManifest.xml"));
            Directory.Move(
                Path.Combine(pathToTheManifests, "Settings.xml"),
                Path.Combine(configPackagePath, "Settings.xml"));

            Directory.Delete(pathToTheManifests);

            ZipFile.CreateFromDirectory(
                Path.Combine(this.PathToCurrentTempFolder, "Temp"),
                codePackagePath + ".zip");
            ZipFile.CreateFromDirectory(
                Path.Combine(configPackagePath),
                configPackagePath + ".zip");
            Directory.Delete(configPackagePath, true);
            Directory.Delete(Path.Combine(this.PathToCurrentTempFolder, "Temp"), true);
        }

        public void DeleteCurrentTempFolder() => Directory.Delete(PathToCurrentTempFolder, true);
    }
}
