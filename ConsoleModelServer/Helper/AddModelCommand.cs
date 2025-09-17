using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
    public class AddModelCommand : CRUDCommand, ICommand
    {
        //dodato
        private readonly ILogger logger;
        public AddModelCommand(ModelRepository repository, Model model, ILogger logger)
            : base(repository, model)
        {
            this.logger = logger;
        }

        public void Execute()
        {
            modelRepository.AddModel(model);
            logger?.Log($"[AddModelCommand] Added model ID: {model.Id}");
        }

        public void Unexecute()
        {
            modelRepository.RemoveModel(model.Id);
            logger?.Log($"[AddModelCommand] Undo: Removed model ID: {model.Id}");
        }
    }
}
