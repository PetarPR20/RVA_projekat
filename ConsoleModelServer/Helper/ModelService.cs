using Common;
using Common.Interface;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Model> SearchModels(
     int? id = null,
     string modelName = null,
     string brandName = null,
     BodyType? bodyType = null,
     int? numberOfDoors = null,
     string engineKeyword = null,
     Type stateType = null)
        {
            var models = repository.GetAllModels();

            var result = models.FindAll(m =>
                (!id.HasValue || m.Id == id.Value) &&
                (string.IsNullOrEmpty(modelName) ||
                    (m.ModelName?.IndexOf(modelName, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                (string.IsNullOrEmpty(brandName) ||
                    (m.BrandName?.IndexOf(brandName, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                (!bodyType.HasValue || m.BodyType == bodyType.Value) &&
                (!numberOfDoors.HasValue || m.NumberOfDoors == numberOfDoors.Value) &&
                (string.IsNullOrEmpty(engineKeyword) ||
                    m.ViableEngines.Exists(e => e.ToString().IndexOf(engineKeyword, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                (stateType == null || m.State.GetType() == stateType)
            );

            if (result.Count == 0)
            {
                logger?.Log("[ModelService] Search returned no results.");
            }
            else
            {
                foreach (var model in result)
                {
                    logger?.Log($"[ModelService] Found -> ID: {model.Id}, Name: {model.ModelName}, Brand: {model.BrandName}, Body: {model.BodyType}, Doors: {model.NumberOfDoors}, Engines: {model.ViableEngines.Count}, State: {model.State.GetType().Name}");
                }
            }

            return result;
        }

    }
}
