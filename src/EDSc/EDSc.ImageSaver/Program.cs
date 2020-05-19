using EDSc.Common.MessageBroker;
using EDSc.Common.Model;
using EDSc.Common.Services;
using EDSc.Common.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using MongoDB.Driver;
using System;
using System.IO;
using System.Threading;

namespace EDSc.ImageSaver
{
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
