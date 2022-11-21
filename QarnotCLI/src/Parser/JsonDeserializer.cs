namespace QarnotCLI
{
    using System.IO;
    using Newtonsoft.Json;

    public interface IDeserializer
    {
        string GetFile(string filename);

        T Deserialize<T>(string jsonFile);

        T GetObjectFromFile<T>(string filePath);
    }

    public class JsonDeserializer : IDeserializer
    {
        private JsonConverter[] Converters { get; set; }

        public JsonDeserializer() { }
        public JsonDeserializer(params JsonConverter[] converters): this()
        {
            Converters = converters;
        }

        public T GetObjectFromFile<T>(string filePath)
        {
            return this.Deserialize<T>(this.GetFile(filePath));
        }

        public string GetFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public T Deserialize<T>(string jsonFile)
        {
            return Converters != default ?
                JsonConvert.DeserializeObject<T>(jsonFile, Converters)
                : JsonConvert.DeserializeObject<T>(jsonFile);
        }
    }
}
