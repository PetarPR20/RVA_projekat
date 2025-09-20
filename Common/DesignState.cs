using Common.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class DesignState : ConcreteState, IStartable, IRedesignable
    {

        public void StartProduction(Model model)
        {
            Console.WriteLine($"Starting production for model {model.ModelName}.");
            model.SetState(new InProductionState());
        }

        public void Redesign(Model model)
        {
            Console.WriteLine($"Redesigning model {model.ModelName}.");
            
        }

    }
}
