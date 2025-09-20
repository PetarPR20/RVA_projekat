using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
    public class RemoveModelCommand : CRUDCommand, ICommand
    {
        //dodato
        private Model removedModel;
        private readonly ILogger logger;

        public RemoveModelCommand(ModelRepository repository, Model model, ILogger logger)
            : base(repository, model)
        {
            this.logger = logger;
        }

        public void Execute()
        {
            removedModel = modelRepository.GetModelById(model.Id);
            if (removedModel != null)
            {
                modelRepository.RemoveModel(model.Id);
                logger?.Log($"[RemoveModelCommand] Removed model ID: {model.Id}");
            }
            else
            {
                logger?.Log($"[RemoveModelCommand] Model ID: {model.Id} not found. Cannot remove.");
            }
        }

        public void Unexecute()
        {
            if (removedModel != null)
            {
                modelRepository.AddModel(removedModel);
                logger?.Log($"[RemoveModelCommand] Undo: Re-added model ID: {removedModel.Id}");
            }
        }
    }
}
