namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using QarnotCLI;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class UnitTestJsonDeserializer : IDeserializer
    {
        public static string GetFileGet { get; set; } = null;

        public static string GetFileRetrun { get; set; } = null;

        public static string DeserializeGet { get; set; } = null;

        public T GetObjectFromFile<T>(string fileName)
        {
            return Deserialize<T>(GetFile(fileName));
        }

        public string GetFile(string fileName)
        {
            Assert.AreEqual(GetFileGet, fileName);
            return GetFileRetrun;
        }

        public T Deserialize<T>(string convertThisString)
        {
            Assert.AreEqual(DeserializeGet, convertThisString);
            return JsonConvert.DeserializeObject<T>(convertThisString);
        }
    }
}