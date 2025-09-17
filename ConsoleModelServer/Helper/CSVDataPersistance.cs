using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleModelServer.Helper
{
    public class CSVDataPersistance : IDataPersistance
    {
        //dodato
        private readonly ILogger logger;

        public CSVDataPersistance(ILogger logger)
        {
            this.logger = logger;
        }
        public List<Model> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                logger?.Log($"[CSV] File not found: {filePath}");
                return new List<Model>();
            }

            try
            {
                var models = new List<Model>();
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var parts = line.Split(',');

                    if (parts.Length >= 5)
                    {
                        var model = new Model(
                            int.Parse(parts[0]),
                            parts[1],
                            parts[2],
                            Enum.TryParse(parts[3], out BodyType bodyType) ? bodyType : BodyType.HATCHBACK,
                            int.Parse(parts[4])
                        );

                        model.SetState(new DesignState());
                        models.Add(model);
                    }
                }

                logger?.Log($"[CSV] Successfully loaded {models.Count} models from {filePath}");
                return models;
            }
            catch (Exception ex)
            {
                logger?.Log($"[CSV] Error loading from {filePath}: {ex.Message}");
                return new List<Model>();
            }
        }

        public void Save(string filePath, List<Model> models)
        {
            try
            {
                var sb = new StringBuilder();

                foreach (var model in models)
                {
                    var modelLine = $"{model.Id},{model.ModelName},{model.BrandName},{model.BodyType},{model.NumberOfDoors}";
                    sb.AppendLine(modelLine);
                }

                File.WriteAllText(filePath, sb.ToString());
                logger?.Log($"[CSV] Successfully saved {models.Count} models to {filePath}");
            }
            catch (Exception ex)
            {
                logger?.Log($"[CSV] Error saving to {filePath}: {ex.Message}");
            }
        }
    }
}
