namespace EDSc.ImageSaver
{
    using Microsoft.ServiceFabric.Services.Runtime;
    using System;
    using System.Threading;
    
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.
                ServiceRuntime.RegisterServiceAsync("ImageSaverManagedServiceType",
                    context => new ImageSaverManagedService(context)).GetAwaiter().GetResult();

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
