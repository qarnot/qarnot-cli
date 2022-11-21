using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QarnotSDK;

namespace QarnotCLI
{
    /// <summary>
    /// Json converter that remove all properties named 'Connection' inside objects of types QTask, QPool, QJob and QBucket.
    ///
    /// Warning: the removal is based on the name of the property and not the type. This supposed that no important nested property is named 'Connection' too
    /// </summary>
    internal class ConnectionJsonConverter : JsonConverter
    {
        private List<Type> TypeWithConnectionToRemove = new List<Type>() { typeof(QTask), typeof(QPool), typeof(QJob), typeof(QBucket)};
        public override bool CanConvert(Type objectType)
        {
            return TypeWithConnectionToRemove.Any(t => objectType == t || objectType.IsSubclassOf(t));
        }
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            var t = JToken.FromObject(value);
            var o = (JObject)t;
            RemoveConnectionProperties(o); // remove the connection property: we don't want to print Connection info in CLI outputs
            o.WriteTo(writer);
        }

        /// <summary>
        /// Remove all properties named 'Connection' recursively.
        /// </summary>
        /// <param name="jObject"></param>
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

        public override Object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(Connection));
        }
    }
}
