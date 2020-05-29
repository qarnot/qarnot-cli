namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleTables;
    using Newtonsoft.Json;

    public interface IResultFormatter
    {
        string Format<T>(T tObj);

        string FormatCollection<T>(List<T> tObjList);
    }

    public class FormatterFactory
    {
        public static string DefaultResultFormat()
        {
            return "TABLE";
        }

        /// <summary>
        /// Format factory
        /// create and return a new format IFormat
        /// or throw an error if there is a unknown format ask.
        /// </summary>
        /// <param name="formatType">format name.</param>
        /// <returns>IFormat object.</returns>
        /// <exception cref="System.MissingMethodException">Thrown when format found <paramref name="formatType"/> is unknown.</exception>
        public static IResultFormatter CreateFormat(string formatType)
        {
            switch (formatType)
            {
                case "TABLE":
                    return new TableFormatter();
                case "JSON":
                    return new JsonFormatter();
                case "XML":
                    return new XMLFormatter();
                default:
                    throw new MissingMethodException(formatType);
            }
        }

        /// <summary>
        /// Check Format factory
        /// create the format ask
        /// throw an error if there is a unknown format ask.
        /// </summary>
        /// <param name="format">format name.</param>
        /// <returns>Return alway true.</returns>
        /// <exception cref="System.MissingMethodException">Thrown when format found <paramref name="format"/> is unknown.</exception>
        public static bool CheckFormat(string format)
        {
            switch (format)
            {
                case "TABLE":
                    return true;
                case "JSON":
                    return true;
                case "XML":
                    return true;
                default:
                    throw new ParseException();
            }
        }

        /// <summary>
        /// Format Table
        /// print a table if a list of ICommandValue is send
        /// print a list if a ICommandValue is send
        /// print a Json if a raw type is send.
        /// </summary>
        public class TableFormatter : JsonFormatter
        {
            public TableFormatter()
            {
            }

            private string FormatObjectCommandValue<T>(List<object> listObj)
                                                        where T : class
            {
                List<T> listVal = listObj.Select(item => item as T).ToList();

                var table = ConsoleTable.From<T>(listVal);
                return table.ToString();
            }

            public override string FormatCollection<T>(List<T> obj)
            {
                if (typeof(ICommandValue).IsAssignableFrom(typeof(T)))
                {
                    var table = ConsoleTable.From<T>(obj);
                    return table.ToString();
                }
                else if (typeof(string).IsAssignableFrom(typeof(T)))
                {
                    var message = "";
                    foreach (var item in obj)
                    {
                        message = string.Join(Environment.NewLine, obj);
                    }
                    return message;
                }

                return CreateJson(obj);
            }

            private string PersonalizeFormatDictionary(Dictionary<string, string> dict)
            {
                string ret = string.Empty;
                foreach (KeyValuePair<string, string> entry in dict)
                {
                    ret += "    " + entry.Key + " : " + entry.Value + Environment.NewLine;
                }

                return ret;
            }

            public string PersonalizeFormatList(List<string> list)
            {
                string ret = string.Empty;
                foreach (string elem in list)
                {
                    ret += "    * " + elem + Environment.NewLine;
                }

                return ret;
            }

            private string FormatCommandValuesForOneElement<T>(System.Reflection.PropertyInfo prop, T obj)
            {
                if (prop.PropertyType == typeof(Dictionary<string, string>))
                {
                    return prop.Name + " : " + Environment.NewLine
                        + PersonalizeFormatDictionary(prop.GetValue(obj) as Dictionary<string, string>);
                }
                else if (prop.PropertyType == typeof(List<string>))
                {
                    return prop.Name + " : " + Environment.NewLine
                        + PersonalizeFormatList(prop.GetValue(obj) as List<string>);
                }
                else
                {
                    return prop.Name + " : " + prop.GetValue(obj) + Environment.NewLine;
                }
            }

            private string FormatObjectCommandValues<T>(T obj)
            {
                Type t = obj.GetType();
                string ret = string.Empty;
                foreach (var prop in t.GetProperties())
                {
                    ret += FormatCommandValuesForOneElement(prop, obj);
                }

                return ret;
            }

            public override string Format<T>(T obj)
            {
                if (typeof(ICommandValue).IsAssignableFrom(typeof(T)))
                {
                    return FormatObjectCommandValues<T>(obj);
                }
                else if (typeof(string).IsAssignableFrom(typeof(T)))
                {
                    return obj as string;
                }

                return CreateJson(obj);
            }
        }

        /// <summary>
        /// Format Json
        /// print the Json format of the object send.
        /// </summary>
        public class JsonFormatter : IResultFormatter
        {
            public JsonFormatter()
            {
            }

            public virtual string FormatCollection<T>(List<T> obj)
            {
                return CreateJson(obj);
            }

            public virtual string Format<T>(T obj)
            {
                return CreateJson(obj);
            }

            protected string CreateJson(object obj)
            {
                try
                {
                    return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
                }
                catch(Newtonsoft.Json.JsonSerializationException ex)
                {
                    CLILogs.Error(ex.Message);
                    return "Value not found";
                }
            }
        }

        /// <summary>
        /// Format XML
        /// print the XML format of the object send.
        /// </summary>
        public class XMLFormatter : IResultFormatter
        {
            public XMLFormatter()
            {
            }

            public string FormatCollection<T>(List<T> obj)
            {
                var value = new Information()
                {
                    Values = obj,
                };

                return CreateXML(value);
            }

            public string Format<T>(T obj)
            {
                return CreateXML(obj);
            }

            private string CreateXML(object obj)
            {
                string jsonSerialize = JsonConvert.SerializeObject(obj);
                CLILogs.Debug(jsonSerialize);
                try
                {
                    return JsonConvert.DeserializeXNode(jsonSerialize, "Information", false).ToString();
                }
                catch(Newtonsoft.Json.JsonSerializationException ex)
                {
                    CLILogs.Error(ex.Message);
                    return "Value not found";
                }
            }

            private class Information
            {
                public object Values { get; set; }
            }
        }
    }
}
