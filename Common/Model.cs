using Common.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;


namespace Common
{
    [DataContract]
    public class Model
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string ModelName { get; set; }

        [DataMember]
        public string BrandName { get; set; }

        [DataMember]
        public BodyType BodyType { get; set; }


        [DataMember]
        public int NumberOfDoors { get; set; }

        [DataMember]
        public List<Engine> ViableEngines { get; set; } = new List<Engine>();

        [JsonIgnore]
        [IgnoreDataMember]
        public ConcreteState State { get; set; }

        [DataMember]
        public string StateName { get; set; }

        public Model(int id, string modelName, string brandName, BodyType bodyType, int numberOfDoors)
        {
            Id = id;
            ModelName = modelName;
            BrandName = brandName;
            BodyType = bodyType;
            NumberOfDoors = numberOfDoors;
            State = new DesignState();
            StateName = State?.GetType().Name ?? "Unknown";
        }

        public Model() { }

        public void StartProduction()
        {
            if (State is IStartable startableState)
            {
                startableState.StartProduction(this);
            }
            else
            {
                Console.WriteLine("Model is not in a state where production can be started.");
            }
        }

        public void StopProduction()
        {
            if (State is IStopable stopableState)
            {
                stopableState.StopProduction(this);
            }
            else
            {
                Console.WriteLine("Model is not in a state where production can be stopped.");
            }
        }

        public void Redesign()
        {
            if (State is IRedesignable redesignableState)
            {
                redesignableState.Redesign(this);
            }
            else
            {
                Console.WriteLine("Model cannot be redesigned in its current state.");
            }
        }

        public void ProductionFailed()
        {
            if (State is IStopable stopableState)
            {
                stopableState.StopProduction(this);
            }
            else
            {
                Console.WriteLine("Model cannot be redesigned in its current state.");
            }
        }

        public void SetState(ConcreteState newState)
        {
            State = newState;
        }

        public void ModelRollOut()
        {
            Console.WriteLine($"Model ID: {Id}");
            Console.WriteLine($"Model Name: {ModelName}");
            Console.WriteLine($"Brand Name: {BrandName}");
            Console.WriteLine($"Body Type: {BodyType.ToString()}");
            Console.WriteLine($"Number of Doors: {NumberOfDoors}");
            Console.WriteLine($"Current State: {State.GetType().Name}");

            Console.WriteLine("Viable Engines:");
            foreach (var engine in ViableEngines)
            {
                engine.EngineSpecs();
            }
        }
    }
}
