using Common.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;


namespace Common
{
    [DataContract]
    public class Model : INotifyPropertyChanged
    {
        private string stateName;
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
        public string StateName
        {
            get => stateName;
            set => SetField(ref stateName, value);
        }

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

        #region
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
