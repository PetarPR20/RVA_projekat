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
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleModelServer
{
    internal class Program
    {
        private static readonly string modelFileJson = "Persistance\\models.json";
        private static readonly string modelFileXml = "Persistance\\models.xml";
        private static readonly string modelFileCsv = "Persistance\\models.csv";

        private static ModelRepository modelRepository;
        private static List<Model> models;
        static void Main(string[] args)
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(exeDir);
            string modelDir = dir.Parent.Parent.Parent.FullName;

            Uri baseAddress = new Uri("net.tcp://localhost:9000/MyService"); // note net.tcp

            Console.WriteLine("Choose persistence format:");
            Console.WriteLine("1. JSON");
            Console.WriteLine("2. XML");
            Console.WriteLine("3. CSV");
            Console.Write("Enter choice (1-3): ");

            string choice = Console.ReadLine();
            IDataPersistance persistance;
            string modelFile;

            ILogger logger = new TxtLogger();

            switch (choice)
            {
                case "2":
                    persistance = new XMLDataPersistance(logger);
                    modelFile = modelFileXml;
                    Console.WriteLine("Using XML persistence.");
                    break;
                case "3":
                    persistance = new CSVDataPersistance(logger);
                    modelFile = modelFileCsv;
                    Console.WriteLine("Using CSV persistence.");
                    break;
                default:
                    persistance = new JSONDataPersistance(logger);
                    modelFile = modelFileJson;
                    Console.WriteLine("Using JSON persistence (default).");
                    break;
            }

            string modelPath = Path.Combine(modelDir, modelFile);


            string modelPathJSON = Path.Combine(modelDir, modelFileJson);
            string modelPathXML = Path.Combine(modelDir, modelFileXml);
            string modelPathCSV = Path.Combine(modelDir, modelFileCsv);

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

                Console.WriteLine("Loading data from " + modelPath);
                models = modelRepository.Load(modelPath);

                // after you load your models in Main
                Task.Run(() =>
                {
                    Random rnd = new Random();

                    while (true) // infinite loop, will run until app exits
                    {
                        foreach (var model in models)
                        {
                            lock (model) // optional if other threads access models
                            {
                                // randomly choose a state-changing method for demonstration
                                int action = rnd.Next(4);
                                switch (action)
                                {
                                    case 0:
                                        model.StartProduction();
                                        break;
                                    case 1:
                                        model.StopProduction();
                                        break;
                                    case 2:
                                        model.Redesign();
                                        break;
                                    case 3:
                                        model.ProductionFailed();
                                        break;
                                }

                                // update the StateName property for display or serialization
                                model.StateName = model.State?.GetType().Name ?? "Unknown";
                            }
                        }

                        Thread.Sleep(2000); // wait 2 seconds
                    }
                });

                Console.WriteLine(modelPath);

                foreach (var model in models)
                {
                    model.ModelRollOut();
                }


                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    
                    Console.WriteLine("Models have been saved before exit.");
                };
                IDataPersistance p1 = new XMLDataPersistance(logger);
                IDataPersistance p2 = new CSVDataPersistance(logger);
                ModelRepository m1 = new ModelRepository(p1, logger);
                host.Open();
                Console.WriteLine("Service is running...");
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                Console.WriteLine(modelPath);
                modelRepository.Save(modelPath);
                List<Model> m = modelRepository.GetAllModels();
                p1.Save(modelPathXML, m);
                p2.Save(modelPathCSV, m);
                Console.ReadKey();
            }

            
        }
    }
}
