using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ConsoleModelServer.Helper
{
    public class JSONDataPersistance : IDataPersistance
    {
        //dodato
        private readonly ILogger logger;

        public JSONDataPersistance(ILogger logger)
        {
            this.logger = logger;
        }
        public List<Model> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                logger?.Log($"[JSON] File not found: {filePath}");
                return new List<Model>();
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var json = File.ReadAllText(filePath);
                var models = JsonSerializer.Deserialize<List<Model>>(json, options);

                foreach (var model in models)
                {
                    model.SetState(new DesignState());
                }

                logger?.Log($"[JSON] Successfully loaded {models.Count} models from {filePath}");
                return models;
            }
            catch (Exception ex)
            {
                logger?.Log($"[JSON] Error loading from {filePath}: {ex.Message}");
                return new List<Model>();
            }
        }

        public void Save(string filePath, List<Model> models)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IgnoreReadOnlyProperties = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };

                var copy = new List<Model>();
                foreach (var model in models)
                {
                    var clone = new Model(model.Id, model.ModelName, model.BrandName, model.BodyType, model.NumberOfDoors);
                    clone.ViableEngines = model.ViableEngines;
                    copy.Add(clone);
                }

                var json = JsonSerializer.Serialize(copy, options);
                File.WriteAllText(filePath, json);
                logger?.Log($"[JSON] Successfully saved {copy.Count} models to {filePath}");
            }
            catch (Exception ex)
            {
                logger?.Log($"[JSON] Error saving to {filePath}: {ex.Message}");
            }
        }
    }
}
