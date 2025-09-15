using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
    public class OutOfProductionState : ConcreteState, IRedesignable
    {
        public void Redesign(Model model)
        {
            throw new NotImplementedException();
        }
    }
}
