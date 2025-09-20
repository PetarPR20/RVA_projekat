using Common;
using ConsoleModelServer.Helper;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));


            ILogger logger = new TxtLogger();
            IDataPersistance persistance = new JSONDataPersistance();
            modelRepository = new ModelRepository(persistance, logger);

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

            Console.WriteLine("Model server started. Press any key to exit...");

            Console.ReadKey();
        }
    }
}
