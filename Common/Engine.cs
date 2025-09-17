using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public class Engine
	{
		private EngineConfiguration configuration;
		private FuelType fuel;
		private int numberOfCylinders;
		private double power;
		private double torque;

        // Public properties for serialization and access
        public EngineConfiguration Configuration { get; set; }
        public FuelType Fuel { get; set; }
        public int NumberOfCylinders { get; set; }
        public double Power { get; set; }
        public double Torque { get; set; }

        public Engine()
        {
            // Prazan konstruktor za serijalizaciju
        }

        public Engine(EngineConfiguration configuration, FuelType fuel, int numberOfCylinders, double power, double torque)
        {
            this.configuration = configuration;
            this.fuel = fuel;
            this.numberOfCylinders = numberOfCylinders;
            this.power = power;
            this.torque = torque;
        }

        public void EngineSpecs()
        {
            Console.WriteLine("Engine Specifications:");
            Console.WriteLine($"Configuration: {configuration}");
            Console.WriteLine($"Fuel Type: {fuel}");
            Console.WriteLine($"Number of Cylinders: {numberOfCylinders}");
            Console.WriteLine($"Power: {power} kW");
            Console.WriteLine($"Torque: {torque} Nm");
        }
    }
}
