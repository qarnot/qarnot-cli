using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QarnotSDK;

namespace QarnotCLI
{
    public class TimePeriodSpecificationJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimePeriodSpecification) || objectType.IsSubclassOf(typeof(TimePeriodSpecification));
        }
        public override bool CanWrite => false;

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override Object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            try
            {
                var jo = JObject.Load(reader);
                var discriminatorField = jo.Value<string>(nameof(TimePeriodSpecification.Type).ToLower()) ??
                    jo.Value<string>(nameof(TimePeriodSpecification.Type));
                switch (discriminatorField)
                {
                    case "Always":
                        return jo.ToObject<TimePeriodAlways>();
                    case "Weekly":
                        return jo.ToObject<TimePeriodWeeklyRecurring>();
                    default:
                        return default;
                };
            } catch (Exception ex) {
                Console.WriteLine($"Failed to deserialize TimePeriodSpecification: {ex.Message}", ex);
                throw;
            }
        }
    }
}