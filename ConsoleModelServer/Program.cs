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
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

            Console.ReadKey();
        }
    }
}
