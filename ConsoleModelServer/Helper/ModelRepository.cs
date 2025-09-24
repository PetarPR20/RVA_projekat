using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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
            model.SetState(new DesignState());
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
                model.SetState(new DesignState());
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
                    model.StateName = model.State.GetType().Name;
                }
                models = loaded;
                return loaded;
            }

            return new List<Model>();
        }

		public void Save(string filePath)
		{
            persistance?.Save(filePath, models);
        }

        public List<Model> SearchModels(
           string modelName = null,
           string brandName = null,
           BodyType? bodyType = null,
           int? numberOfDoors = null)
        {
            return models.Where(model =>
                (string.IsNullOrEmpty(modelName) ||
                (model.ModelName != null && model.ModelName.IndexOf(modelName, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                (string.IsNullOrEmpty(brandName) ||
                (model.BrandName != null && model.BrandName.IndexOf(brandName, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                (!bodyType.HasValue || model.BodyType == bodyType) &&
                (!numberOfDoors.HasValue || model.NumberOfDoors == numberOfDoors)
                
            ).ToList();
        }
    }
}
