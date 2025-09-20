using Common.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class OutOfProductionState : ConcreteState, IRedesignable
    {
        public void Redesign(Model model)
        {
            Console.WriteLine($"Redesigning model {model.ModelName} in OutOfProductionState.");
            model.SetState(new DesignState());
        }
    }
}
