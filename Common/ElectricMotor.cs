using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public class ElectricMotor : EMobility
	{
		private double power;
		private double torque;
		private string cooling;
		private double batteryCapacity;

        public ElectricMotor()
        {
            // Prazan konstruktor za serijalizaciju
        }

        public ElectricMotor(double voltage, PowerSource source, double power, double torque, string cooling, double batteryCapacity)
        {
            this.voltage = voltage;
            this.source = source;
            Power = power;
            Torque = torque;
            Cooling = cooling;
            BatteryCapacity = batteryCapacity;
        }

        public double Power { get; private set; }
        public double Torque { get; private set; }
        public string Cooling { get; private set; }

        private double BatteryCapacity;

        public override void EVMotorSpecs()
        {
            Console.WriteLine($"Electric Motor Specs:");
            Console.WriteLine($"Voltage: {voltage} V");
            Console.WriteLine($"Power Source: {source}");
            Console.WriteLine($"Power: {Power} kW");
            Console.WriteLine($"Torque: {Torque} Nm");
            Console.WriteLine($"Cooling System: {Cooling}");
            Console.WriteLine($"Battery Capacity: {BatteryCapacity} kWh");
        }
    }
}
