using ConsoleModelServer.Interface;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ConsoleModelServer.Helper
{
    public class TxtLogger : ILogger
    {
        //dodato
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static TxtLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public void Log(string message)
        {
            log.Info(message);
        }
    }
}
