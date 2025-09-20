using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
	public class ModelRepository
	{
        private List<Model> models = new List<Model>();
        private IDataPersistance persistance;
        private ILogger logger;

        public ModelRepository(IDataPersistance persistance, ILogger logger)
        {
            this.persistance = persistance;
            this.logger = logger;
        }

        public void AddModel(Model model)
		{
            if (models.Exists(m => m.Id == model.Id))
            {
                logger?.Log($"Model with ID {model.Id} already exists.");
                return;
            }
            models.Add(model);
            logger?.Log($"Model with ID {model.Id} added.");
        }

		public void RemoveModel(int id)
		{
            var model = models.Find(m => m.Id == id);
            if (model != null)
            {
                models.Remove(model);
                logger?.Log($"Model with ID {id} removed.");
            }
            else
            {
                logger?.Log($"Model with ID {id} not found for removal.");
            }
        }

		public void UpdateModel(Model model)
		{
            var index = models.FindIndex(m => m.Id == model.Id);
            if (index >= 0)
            {
                models[index] = model;
                logger?.Log($"Model with ID {model.Id} updated.");
            }
            else
            {
                logger?.Log($"Model with ID {model.Id} not found for update.");
            }
        }

        //dodato
        public Model GetModelById(int id)
        {
            return models.Find(m => m.Id == id);
        }

        //dodato
        public List<Model> GetAllModels()
        {
            return new List<Model>(models);
        }

        public List<Model> Load(string filePath)
		{

            var loaded = persistance?.Load(filePath);
            if (loaded != null)
            {
                foreach (var model in loaded)
                {
                    model.SetState(new DesignState());
                }
                return loaded;
            }

            return new List<Model>();
        }

		public void Save(string filePath, List<Model> models)
		{
            persistance?.Save(filePath, models);
        }
	}
}
