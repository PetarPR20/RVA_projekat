using Common.Interface;
using System;
using System.Collections.Generic;
using System.Text;


namespace Common
{
    public class Model
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public string BrandName { get; set; }
        public BodyType BodyType { get; set; }
        public int NumberOfDoors { get; set; }
        public List<Engine> ViableEngines { get; set; } = new List<Engine>();
        public ConcreteState State { get; set; }

        public Model(int id, string modelName, string brandName, BodyType bodyType, int numberOfDoors)
        {
            Id = id;
            ModelName = modelName;
            BrandName = brandName;
            BodyType = bodyType;
            NumberOfDoors = numberOfDoors;
            State = new DesignState();
        }

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
