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
        private readonly ILogger logger;

        public CSVDataPersistance(ILogger logger)
        {
            this.logger = logger;
        }

        public void Save(string filePath, List<Model> models)
        {
            try
            {
                var sb = new StringBuilder();

                // Header row
                sb.AppendLine("ModelId,ModelName,BrandName,BodyType,NumberOfDoors,EngineType,Configuration,Fuel,Cylinders,Power,Torque,EVVoltage,EVBattery,Cooling,Source");

                foreach (var model in models)
                {
                    foreach (var engine in model.ViableEngines)
                    {
                        string engineType = engine.GetType().Name;
                        string line = $"{model.Id},{model.ModelName},{model.BrandName},{model.BodyType},{model.NumberOfDoors},";

                        if (engine is ElectricMotorAdapter ev)
                        {
                            line += $"EVAdapter,{ev.Configuration},{ev.Fuel},{ev.NumberOfCylinders},{ev.Power},{ev.Torque},{ev.Voltage},{ev.BatteryCapacity},{ev.Cooling},{ev.Source}";
                        }
                        else
                        {
                            line += $"Engine,{engine.Configuration},{engine.Fuel},{engine.NumberOfCylinders},{engine.Power},{engine.Torque},,,,,";
                        }

                        sb.AppendLine(line);
                    }
                }

                File.WriteAllText(filePath, sb.ToString());
                logger?.Log($"[CSV] Successfully saved {models.Count} models to {filePath}");
            }
            catch (Exception ex)
            {
                logger?.Log($"[CSV] Error saving to {filePath}: {ex.Message}");
            }
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
                var models = new Dictionary<int, Model>();
                var lines = File.ReadAllLines(filePath).Skip(1); // skip header

                foreach (var line in lines)
                {
                    var parts = line.Split(',');

                    if (parts.Length < 6)
                        continue; // invalid row

                    int id = int.Parse(parts[0]);
                    string modelName = parts[1];
                    string brandName = parts[2];
                    BodyType bodyType = Enum.TryParse(parts[3], out BodyType bt) ? bt : BodyType.HATCHBACK;
                    int numberOfDoors = int.Parse(parts[4]);
                    string engineType = parts[5];

                    // Get or create model
                    if (!models.TryGetValue(id, out var model))
                    {
                        model = new Model(id, modelName, brandName, bodyType, numberOfDoors);
                        model.ViableEngines = new List<Engine>();
                        model.SetState(new DesignState());
                        models[id] = model;
                    }

                    if (engineType == "Engine")
                    {
                        var engine = new Engine
                        {
                            Configuration = Enum.TryParse(parts[6], out EngineConfiguration ec) ? ec : EngineConfiguration.INLINE,
                            Fuel = Enum.TryParse(parts[7], out FuelType ft) ? ft : FuelType.GASOLINE,
                            NumberOfCylinders = int.TryParse(parts[8], out int cyl) ? cyl : 0,
                            Power = double.TryParse(parts[9], out double p) ? p : 0,
                            Torque = double.TryParse(parts[10], out double t) ? t : 0
                        };
                        model.ViableEngines.Add(engine);
                    }
                    else if (engineType == "EVAdapter")
                    {
                        var ev = new ElectricMotorAdapter(new ElectricMotor
                        {
                            Power = double.TryParse(parts[9], out double p) ? p : 0,
                            Torque = double.TryParse(parts[10], out double t) ? t : 0,
                            Voltage = double.TryParse(parts[11], out double v) ? v : 0,
                            BatteryCapacity = double.TryParse(parts[12], out double b) ? b : 0,
                            Cooling = parts[13],
                            Source = Enum.TryParse(parts[14], out PowerSource ps) ? ps : PowerSource.AC
                        });

                        // Map common engine fields
                        ev.Configuration = Enum.TryParse(parts[6], out EngineConfiguration ec2) ? ec2 : EngineConfiguration.INLINE;
                        ev.Fuel = Enum.TryParse(parts[7], out FuelType ft2) ? ft2 : FuelType.HYBRID;
                        ev.NumberOfCylinders = int.TryParse(parts[8], out int cyl2) ? cyl2 : 0;

                        model.ViableEngines.Add(ev);
                    }
                }

                logger?.Log($"[CSV] Successfully loaded {models.Count} models from {filePath}");
                return models.Values.ToList();
            }
            catch (Exception ex)
            {
                logger?.Log($"[CSV] Error loading from {filePath}: {ex.Message}");
                return new List<Model>();
            }
        }
    }
}
