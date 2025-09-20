using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ConsoleModelServer.Helper
{
    public class XMLDataPersistance : IDataPersistance
    {
        //dodato
        private readonly ILogger logger;

        public XMLDataPersistance(ILogger logger)
        {
            this.logger = logger;
        }
        public List<Model> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                logger?.Log($"[XML] File not found: {filePath}");
                return new List<Model>();
            }

            try
            {
                var serializer = new XmlSerializer(typeof(List<Model>));

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var models = (List<Model>)serializer.Deserialize(stream);

                    foreach (var model in models)
                    {
                        model.SetState(new DesignState());
                    }

                    logger?.Log($"[XML] Successfully loaded {models.Count} models from {filePath}");
                    return models;
                }
            }
            catch (Exception ex)
            {
                logger?.Log($"[XML] Error loading from {filePath}: {ex.Message}");
                return new List<Model>();
            }
        }

        public void Save(string filePath, List<Model> models)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<Model>));

                // Pravimo kopiju bez stanja
                var copy = new List<Model>();
                foreach (var model in models)
                {
                    var clone = new Model(model.Id, model.ModelName, model.BrandName, model.BodyType, model.NumberOfDoors);
                    clone.ViableEngines = model.ViableEngines;
                    copy.Add(clone);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(stream, copy);
                }

                logger?.Log($"[XML] Successfully saved {copy.Count} models to {filePath}");
            }
            catch (Exception ex)
            {
                logger?.Log($"[XML] Error saving to {filePath}: {ex.Message}");
            }
        }
    }
}
