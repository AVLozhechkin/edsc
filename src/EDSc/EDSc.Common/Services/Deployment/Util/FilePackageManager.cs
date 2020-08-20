namespace EDSc.Common.Services.Deployment.Util
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using EDSc.Common.Services.Deployment.Model;
    using Microsoft.Extensions.Configuration;
    
    public class FilePackageManager : IPackageManager
    {
        private string PathToDropFolder { get; }
        private string PathToBaseTempFolder { get; }

        public FilePackageManager(IConfigurationSection configurationSection)
        {
            this.PathToDropFolder = configurationSection.GetValue<string>("pathToDropFolder");
            this.PathToBaseTempFolder = configurationSection.GetValue<string>("pathToTempFolder");
        }

        public string PathToCurrentTempFolder { get; private set; }

        public void ProvideTempPackageFolder(InstanceDescription instanceDescription)
        {
            this.PathToCurrentTempFolder = Path.Combine(this.PathToBaseTempFolder, Guid.NewGuid().ToString());
            ZipFile.ExtractToDirectory(
                Path.Combine(
                    this.PathToDropFolder, instanceDescription.BuildVersion, 
                    instanceDescription.ApplicationTypeName + ".zip"),
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
