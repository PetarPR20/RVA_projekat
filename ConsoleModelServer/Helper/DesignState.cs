using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
    public class DesignState : ConcreteState, IStartable, IRedesignable
    {
        public void Redesign(Model model)
        {
            throw new NotImplementedException();
        }

        public void StartProduction(Model model)
        {
            throw new NotImplementedException();
        }
    }
}
