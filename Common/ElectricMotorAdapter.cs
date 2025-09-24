using System;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class ElectricMotorAdapter : Engine
    {
        // EV-specific properties for binding & serialization
        [DataMember] public double Voltage { get; set; }
        [DataMember] public double BatteryCapacity { get; set; }
        [DataMember] public string Cooling { get; set; }
        [DataMember] public PowerSource Source { get; set; }

        // Parameterless constructor required for WCF
        public ElectricMotorAdapter() { }

        // Initialize from ElectricMotor
        public ElectricMotorAdapter(ElectricMotor ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));

            // Common Engine properties
            Configuration = EngineConfiguration.INLINE;  // default
            Fuel = FuelType.HYBRID;                     // must exist in enum
            NumberOfCylinders = 0;

            Power = ev.Power;
            Torque = ev.Torque;

            // EV-specific
            Voltage = ev.Voltage;
            BatteryCapacity = ev.BatteryCapacity;
            Cooling = ev.Cooling;
            Source = ev.Source;
        }
    }
}
