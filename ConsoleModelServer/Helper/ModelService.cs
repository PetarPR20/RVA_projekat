using Common;
using Common.Interface;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace ConsoleModelServer.Helper
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ModelService : IModelService
	{
		ModelRepository repository;
		CommandManager commandManager;
        //dodato
        private readonly ILogger logger;

        public ModelService(ModelRepository repository, CommandManager commandManager)
        {
            this.repository = repository;
            this.commandManager = commandManager;
            this.logger = new TxtLogger();
        }

        public List<Model> SearchModels(
           string modelName = null,
           string brandName = null,
           BodyType? bodyType = null,
           int? numberOfDoors = null)
        {
            return repository.SearchModels(modelName, brandName, bodyType, numberOfDoors);
        }

        public bool CanUndo() => commandManager.GetUndoStackCount() > 0;
        public bool CanRedo() => commandManager.GetRedoStackCount() > 0;

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

        public List<Model> GetAllModels()
        {
            List<Model> models = repository.GetAllModels();
            if (models.Count == 0)
            {
                logger?.Log("[ModelService] No models found.");
                return new List<Model>();
            }

            foreach (var model in models)
            {
                logger?.Log($"[ModelService] Model ID: {model.Id}, Name: {model.ModelName}, Brand: {model.BrandName}, State: {model.State.GetType().Name}");
            }

            logger?.Log("[ModelService] All models listed.");
            return models;
        }

        public Model Alive()
        {
            Model model = new Model(7, "asdasd", "asdasdasd", BodyType.SUV, 7);
            model.ViableEngines = new List<Engine>()
            {
                new Engine(EngineConfiguration.INLINE, FuelType.DIESEL, 3, 45, 7)
            };
            return model;
        }

        public void AddEngine(int id, Engine engine)
        {
            Model model = repository.GetModelById(id);
            model.ViableEngines.Add(engine);
            repository.UpdateModel(model);
        }

    }
}
