using Common;
using Common.Interface;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
	public class ModelService : IModelService
	{
		ModelRepository repository;
		CommandManager commandManager;
        //dodato
        private readonly ILogger logger;

        public void AddModel(Model model)
        {
            var cmd = new AddModelCommand(repository, model, logger);
            commandManager.ExecuteCommand(cmd);
            logger?.Log($"[ModelService] AddModelCommand issued for model ID: {model.Id}");
        }

        public void RemoveModel(int id)
        {
            var model = repository.GetModelById(id);
            if (model == null)
            {
                logger?.Log($"[ModelService] Cannot remove - Model with ID {id} not found.");
                return;
            }

            var cmd = new RemoveModelCommand(repository, model, logger);
            commandManager.ExecuteCommand(cmd);
            logger?.Log($"[ModelService] RemoveModelCommand issued for model ID: {id}");
        }

        public void UpdateModel(Model model)
        {
            var existing = repository.GetModelById(model.Id);
            if (existing == null)
            {
                logger?.Log($"[ModelService] Cannot update - Model with ID {model.Id} not found.");
                return;
            }

            var cmd = new UpdateModelCommand(repository, existing, model, logger);
            commandManager.ExecuteCommand(cmd);
            logger?.Log($"[ModelService] UpdateModelCommand issued for model ID: {model.Id}");
        }

        public void Undo()
        {
            commandManager.Undo();
            logger?.Log("[ModelService] Undo executed.");
        }

        public void Redo()
        {
            commandManager.Redo();
            logger?.Log("[ModelService] Redo executed.");
        }

        public void GetAllModels()
        {
            var models = repository.GetAllModels();
            if (models.Count == 0)
            {
                logger?.Log("[ModelService] No models found.");
                return;
            }

            foreach (var model in models)
            {
                logger?.Log($"[ModelService] Model ID: {model.Id}, Name: {model.ModelName}, Brand: {model.BrandName}, State: {model.State.GetType().Name}");
            }

            logger?.Log("[ModelService] All models listed.");
        }
    }
}
