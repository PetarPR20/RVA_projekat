using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Common
{
    [DataContract]
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
            this.Voltage = voltage;
            this.Source = source;
            Power = power;
            Torque = torque;
            Cooling = cooling;
            BatteryCapacity = batteryCapacity;
        }

        [DataMember] public double Power { get;  set; }
        [DataMember] public double Torque { get;  set; }
        [DataMember] public string Cooling { get;  set; }
        [DataMember] public double Voltage { get;  set; }
        [DataMember] public PowerSource Source { get;  set; }

        [DataMember] public double BatteryCapacity { get; set; }

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
