using Common;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class InProductionState : ConcreteState, IStopable, IFailable
    {
        public void StopProduction(Model model)
        {
            Console.WriteLine($"Stopping production for model {model.ModelName}.");
            model.SetState(new OutOfProductionState());
            model.StateName = "OutOfProduction";
        }

        public void ProductionFailed(Model model)
        {
            Console.WriteLine($"Production failed for model {model.ModelName}. Transitioning to DesignState.");
            model.SetState(new DesignState());
            model.StateName = "Design";

        }

    }
}
