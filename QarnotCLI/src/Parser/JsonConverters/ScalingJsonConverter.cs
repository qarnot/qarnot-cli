using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QarnotSDK;

namespace QarnotCLI
{
    public class ScalingPolicyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ScalingPolicy) || objectType.IsSubclassOf(typeof(ScalingPolicy));
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
                var discriminatorField = jo.Value<string>(nameof(ScalingPolicy.Type).ToLower()) ??
                    jo.Value<string>(nameof(ScalingPolicy.Type));

                // This is required so that our other custom converters are used for nested properties (in this case in
                // particular TimePeriodSpecificationConverter), we need to pass a custom serializer to JObject.ToObject().
                // However, we cannot simply pass "serializer", if we do we get back here and recurse endlessly (that is,
                // until a stack overflow).
                var serializerExceptScalingPolicy = new JsonSerializer();
                serializerExceptScalingPolicy.ContractResolver = serializer.ContractResolver;
                serializerExceptScalingPolicy.Formatting = serializer.Formatting;
                foreach (var converter in serializer.Converters) {
                    if (!(converter is ScalingPolicyConverter)) {
                        serializerExceptScalingPolicy.Converters.Add(converter);
                    }
                }

                switch (discriminatorField)
                {
                    case "Fixed":
                        return jo.ToObject<FixedScalingPolicy>(serializerExceptScalingPolicy);
                    case "ManagedTasksQueue":
                        return jo.ToObject<ManagedTasksQueueScalingPolicy>(serializerExceptScalingPolicy);
                    default:
                        return default;
                }
            } catch (Exception ex) {
                Console.WriteLine($"Failed to deserialize Scaling policy: {ex.Message}", ex);
                throw;
            }
        }
    }
}
