using QarnotSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QarnotCLI;

public class TimePeriodSpecificationJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TimePeriodSpecification) || objectType.IsSubclassOf(typeof(TimePeriodSpecification));
    }
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object? value,JsonSerializer serializer) =>
        throw new Exception($"Serialization is not supported for {nameof(TimePeriodSpecificationJsonConverter)}");


    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,JsonSerializer serializer)
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

public class ScalingPolicyConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ScalingPolicy) || objectType.IsSubclassOf(typeof(ScalingPolicy));
    }
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object? value,JsonSerializer serializer) =>
        throw new Exception($"Serialization is not supported for {nameof(ScalingPolicyConverter)}");

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,JsonSerializer serializer)
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

class ConnectionJsonConverter : JsonConverter
{
    private List<Type> TypeWithConnectionToRemove = new List<Type>() { typeof(QTask), typeof(QPool), typeof(QJob), typeof(QBucket) };
    public override bool CanConvert(Type objectType)
    {
        return TypeWithConnectionToRemove.Any(t => objectType == t || objectType.IsSubclassOf(t));
    }

    public override bool CanRead => false;
    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            var val = JValue.CreateNull();
            val.WriteTo(writer);
        }
        else
        {
            var t = JToken.FromObject(value);
            var o = (JObject)t;
            RemoveConnectionProperties(o); // remove the connection property: we don't want to print Connection info in CLI outputs
            o.WriteTo(writer);
        }
    }

    private static void RemoveConnectionProperties(JObject jObject)
    {
        jObject.Remove("Connection");
        List<JProperty> jProperties = jObject.Properties().ToList();
        for (int i = 0; i < jProperties.Count; i++)
        {
            JProperty jProperty = jProperties[i];
            if (jProperty.Value.Type == JTokenType.Array)
            {
                RemoveFromArray((JArray)jProperty.Value);
            }
            else if (jProperty.Value is JObject subObject && subObject.HasValues)
            {
                RemoveConnectionProperties(subObject);
            }
        }
    }
    private static void RemoveFromArray(JArray jArray)
    {
        foreach(JObject jObject in jArray.OfType<JObject>())
        {
            if (jObject.HasValues)
            {
                RemoveConnectionProperties(jObject);
            }
        }
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,JsonSerializer serializer) =>
        throw new Exception($"Deserialization is not supported for {nameof(ConnectionJsonConverter)}");
}
