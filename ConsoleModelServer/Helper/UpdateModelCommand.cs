using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
	public class UpdateModelCommand : CRUDCommand, ICommand
    {
        //dodato
        private readonly Model newModel;
        private Model oldModel;
        private readonly ILogger logger;

        public UpdateModelCommand(ModelRepository repository, Model oldModel, Model newModel, ILogger logger)
            : base(repository, oldModel)
        {
            this.oldModel = DeepCopy(oldModel);
            this.newModel = newModel;
            this.logger = logger;
        }

        public void Execute()
        {
            modelRepository.UpdateModel(newModel);
            logger?.Log($"[UpdateModelCommand] Updated model ID: {newModel.Id}");
        }

        public void Unexecute()
        {
            modelRepository.UpdateModel(oldModel);
            logger?.Log($"[UpdateModelCommand] Undo: Restored previous state for model ID: {oldModel.Id}");
        }

        //dodato
        private Model DeepCopy(Model model)
        {
            
            return new Model(model.Id, model.ModelName, model.BrandName, model.BodyType, model.NumberOfDoors)
            {
                ViableEngines = new List<Engine>(model.ViableEngines),
                State = model.State
            };
        }
    }
}
