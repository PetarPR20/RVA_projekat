using Common;
using Common.Interface;
using ConsoleModelServer.Helper;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleModelServer
{
    internal class Program
    {
        private static readonly string modelFile = "Models.txt"; // JSON format
        private static ModelRepository modelRepository;
        private static List<Model> models;
        static void Main(string[] args)
        {

            // Base address for the service
            Uri baseAddress = new Uri("net.tcp://localhost:9000/MyService"); // note net.tcp

            ILogger logger = new TxtLogger();
            IDataPersistance persistance = new JSONDataPersistance(logger);
            modelRepository = new ModelRepository(persistance, logger);
            CommandManager commandManager = new CommandManager(logger);

            var ServiceInstance = new ModelService(modelRepository, commandManager);

            // Create the ServiceHost
            using (ServiceHost host = new ServiceHost(ServiceInstance, baseAddress))
            {
                var binding = new NetTcpBinding(SecurityMode.None)
                {
                    MaxReceivedMessageSize = 10_000_000, // 10 MB
                    MaxBufferSize = 10_000_000,
                    SendTimeout = TimeSpan.FromMinutes(5),
                    ReceiveTimeout = TimeSpan.FromMinutes(5)
                };
                // Add a service endpoint
                host.AddServiceEndpoint(typeof(IModelService), binding, "");

                log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

                models = modelRepository.Load(modelFile);

                Console.WriteLine($"Loaded {models.Count} model(s).");

                foreach (var model in models)
                {
                    model.ModelRollOut();
                }


                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    modelRepository.Save(modelFile, models);
                    Console.WriteLine("Models have been saved before exit.");
                };

                host.Open();
                Console.WriteLine("Service is running...");
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }

            
        }
    }
}
