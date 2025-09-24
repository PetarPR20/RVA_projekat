using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common;

public class EngineConverter : JsonConverter<Engine>
{
    public override Engine Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        Engine engine = null;

        double power = 0;
        double torque = 0;
        EngineConfiguration configuration = EngineConfiguration.INLINE;
        FuelType fuel = FuelType.DIESEL;
        int cylinders = 0;

        // EV-specific
        double voltage = 0;
        double battery = 0;
        string cooling = null;
        PowerSource? source = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            string propName = reader.GetString();
            reader.Read(); // Move to value

            switch (propName)
            {
                case "Configuration":
                    if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int conf))
                        configuration = (EngineConfiguration)conf;
                    break;
                case "Fuel":
                    if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int f))
                        fuel = (FuelType)f;
                    break;
                case "NumberOfCylinders":
                    if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int c))
                        cylinders = c;
                    break;
                case "Power":
                    if (reader.TokenType == JsonTokenType.Number && reader.TryGetDouble(out double p))
                        power = p;
                    break;
                case "Torque":
                    if (reader.TokenType == JsonTokenType.Number && reader.TryGetDouble(out double t))
                        torque = t;
                    break;

                // EV-specific
                case "Voltage":
                    if (reader.TokenType == JsonTokenType.Number && reader.TryGetDouble(out double v))
                        voltage = v;
                    break;
                case "BatteryCapacity":
                    if (reader.TokenType == JsonTokenType.Number && reader.TryGetDouble(out double b))
                        battery = b;
                    break;
                case "Cooling":
                    if (reader.TokenType == JsonTokenType.String)
                        cooling = reader.GetString();
                    break;
                case "Source":
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        if (Enum.TryParse<PowerSource>(reader.GetString(), out var s))
                            source = s;
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                        source = (PowerSource)reader.GetInt32();
                    break;
            }
        }

        // Decide if this is EV or regular engine
        bool isEV = voltage > 0 && battery > 0 && cooling != null && source.HasValue;

        if (isEV)
        {
            var evMotor = new ElectricMotor(voltage, source.Value, power, torque, cooling, battery);
            var adapter = new ElectricMotorAdapter(evMotor);
            adapter.BatteryCapacity = battery;
            adapter.Voltage = voltage;
            adapter.Cooling = cooling;
            adapter.Source = source.Value;
            engine = adapter;
        }
        else
        {
            engine = new Engine(configuration, fuel, cylinders, power, torque);
        }

        return engine;
    }

    public override void Write(Utf8JsonWriter writer, Engine value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // Common fields
        writer.WriteNumber("Configuration", (int)value.Configuration);
        writer.WriteNumber("Fuel", (int)value.Fuel);
        writer.WriteNumber("NumberOfCylinders", value.NumberOfCylinders);
        writer.WriteNumber("Power", value.Power);
        writer.WriteNumber("Torque", value.Torque);

        // EV-specific
        if (value is ElectricMotorAdapter ev)
        {
            writer.WriteNumber("Voltage", ev.Voltage);
            writer.WriteNumber("BatteryCapacity", ev.BatteryCapacity);
            writer.WriteString("Cooling", ev.Cooling);
            writer.WriteString("Source", ev.Source.ToString());
        }

        writer.WriteEndObject();
    }
}
