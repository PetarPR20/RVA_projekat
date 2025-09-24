using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        /*public List<Model> Load(string filePath)
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
        }*/

        public List<Model> Load(string filePath)
        {
            try
            {
                string fullPath = Path.GetFullPath(filePath);

                if (!File.Exists(fullPath))
                {
                    logger?.Log($"[JSON] File not found: {fullPath}, returning empty list.");
                    return new List<Model>();
                }

                string json = File.ReadAllText(fullPath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                var models = JsonSerializer.Deserialize<List<Model>>(json, options) ?? new List<Model>();
                logger?.Log($"[JSON] Successfully loaded {models.Count} models from {fullPath}");
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
                string fullPath = Path.GetFullPath(filePath);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IgnoreReadOnlyProperties = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };

                var copy = models.Select(model => new Model(model.Id, model.ModelName, model.BrandName, model.BodyType, model.NumberOfDoors)
                {
                    ViableEngines = model.ViableEngines
                }).ToList();

                var json = JsonSerializer.Serialize(copy, options);
                File.WriteAllText(fullPath, json);

                logger?.Log($"[JSON] Successfully saved {copy.Count} models to {fullPath}");
            }
            catch (Exception ex)
            {
                logger?.Log($"[JSON] Error saving to {filePath}: {ex.Message}");
            }
        }


    }
}
