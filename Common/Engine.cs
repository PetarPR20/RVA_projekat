using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Common
{
    [DataContract]
    [KnownType(typeof(ElectricMotorAdapter))]
    [XmlInclude(typeof(ElectricMotorAdapter))]
    public class Engine
	{
		private EngineConfiguration configuration;
		private FuelType fuel;
		private int numberOfCylinders;
		private double power;
		private double torque;

        // Public properties for serialization and access
        [DataMember]
        public EngineConfiguration Configuration { get; set; }
        [DataMember]
        public FuelType Fuel { get; set; }
        [DataMember]
        public int NumberOfCylinders { get; set; }
        [DataMember]
        public double Power { get; set; }
        [DataMember]
        public double Torque { get; set; }
            

        public Engine()
        {
            // Prazan konstruktor za serijalizaciju
        }

        public Engine(EngineConfiguration configuration, FuelType fuel, int numberOfCylinders, double power, double torque)
        {
            Configuration = configuration;
            Fuel = fuel;
            NumberOfCylinders = numberOfCylinders;
            Power = power;
            Torque = torque;
        }

        public void EngineSpecs()
        {
            Console.WriteLine("Engine Specifications:");
            Console.WriteLine($"Configuration: {Configuration}");
            Console.WriteLine($"Fuel Type: {Fuel}");
            Console.WriteLine($"Number of Cylinders: {NumberOfCylinders}");
            Console.WriteLine($"Power: {Power} kW");
            Console.WriteLine($"Torque: {Torque} Nm");
        }
    }
}
