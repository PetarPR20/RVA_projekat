using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
    public class InProductionState : ConcreteState, IStopable, IFailable
    {
        public void ProductionFailed(Model model)
        {
            throw new NotImplementedException();
        }

        public void StopProduction(Model model)
        {
            throw new NotImplementedException();
        }
    }
}
