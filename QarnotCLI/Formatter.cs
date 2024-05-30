using System.Text;
using System.Reflection;
using ConsoleTables;
using Newtonsoft.Json;
using QarnotSDK;

namespace QarnotCLI;

public interface IFormatter
{
    string Format<T>(T tObj);
    string FormatCollection<T>(ICollection<T> tObjList);
    string FormatCollectionPage<T>(ResourcesPageModel<T> tObjList);
}

public interface IFormatterFactory
{
    IFormatter Create(string format);
}

public class FormatterFactory : IFormatterFactory
{
    public IFormatter Create(string format) =>
        format switch {
            "TABLE" => new TableFormatter(),
            "JSON" => new JsonFormatter(),
            _ => throw new MissingMethodException(format),
        };

}

public class TableFormatter : IFormatter
{
    private readonly JsonFormatter JsonFormatter;

    public TableFormatter()
    {
        JsonFormatter = new();
    }

    public string FormatCollection<T>(ICollection<T> obj)
    {
        if (typeof(T).Equals(typeof(HardwareConstraint)))
        {
            var summary = BindHardwareConstraints((ICollection<HardwareConstraint>) obj);
            return ConsoleTable.From(summary).ToString();
        }

        if (IsQarnotSDKType<T>())
        {
            return JsonFormatter.Format(obj);
        }

        if (typeof(string).IsAssignableFrom(typeof(T)))
        {
            return obj.Aggregate(
                new StringBuilder(),
                (b, o) => b.AppendLine(o?.ToString() ?? "")
            ).ToString();
        }

        return ConsoleTable.From<T>(obj).ToString();
    }

    public string Format<T>(T obj)
    {
        if (IsQarnotSDKType<T>())
        {
            return JsonFormatter.Format(obj);
        }

        if (typeof(string).IsAssignableFrom(typeof(T)))
        {
            return obj as string ?? "";
        }

        var builder = typeof(T).GetProperties().Aggregate(
            new StringBuilder(),
            (b, prop) => FormatProperty(prop, obj, b)
        );

        return builder.ToString();
    }

    private StringBuilder FormatProperty<T>(PropertyInfo prop, T obj, StringBuilder builder)
    {
        builder.Append(prop.Name);
        builder.Append(" : ");

        var value = prop.GetValue(obj);
        switch (value)
        {
            case Dictionary<string, string> dict:
                builder.AppendLine();
                return dict.Aggregate(
                    builder,
                    (b, kvp) => b.AppendFormat("    {0} : {1}", kvp.Key, kvp.Value).AppendLine()
                );

            case List<string> list:
                builder.AppendLine();
                return list.Aggregate(
                    builder,
                    (b, elt) => b.AppendFormat("    * {0}", elt).AppendLine()
                );

            default:
                builder.Append(value);
                builder.AppendLine();
                return builder;
        }
    }

    private static bool IsQarnotSDKType<T>() =>
        typeof(T).Namespace?.Contains(nameof(QarnotSDK)) ?? false;

    public string FormatCollectionPage<T>(ResourcesPageModel<T> obj)
    {
        if (IsQarnotSDKType<T>())
        {
            return JsonFormatter.Format(obj);
        }

        if (typeof(T).IsAssignableFrom(typeof(string)))
        {
            return obj.Items.Aggregate(
                new StringBuilder(),
                (b, o) => b.AppendLine(o?.ToString() ?? "")
            ).ToString();
        }

        var consoleTable = ConsoleTable.From<T>(obj.Items);
        var formattedTable = consoleTable.ToString();

        if (!string.IsNullOrWhiteSpace(obj.NextPageToken))
        {
            formattedTable += $"{Environment.NewLine} Next page token: {obj.NextPageToken}";
        }

        return formattedTable;
    }

    private static IEnumerable<ListHardwareConstraintsSummaryItem> BindHardwareConstraints(IEnumerable<HardwareConstraint> constraints)
    {
        return constraints
                .Where(cons => cons is not null)
                .Select(constraint => new ListHardwareConstraintsSummaryItem(
                    Name: constraint.Discriminator,
                    Options: string.Join(", ",
                                    constraint.GetType()
                                        .GetProperties()
                                        .Where(prop => prop.Name != nameof(HardwareConstraint.Discriminator))
                                        .Select(prop => $"{prop.Name}: {prop.GetValue(constraint)}"))
                ));
    }
}

public class JsonFormatter : IFormatter
{
    private readonly JsonConverter[] Converters;

    public JsonFormatter()
    {
        Converters = new JsonConverter[] {
            new ConnectionJsonConverter(),
        };
    }

    public virtual string FormatCollection<T>(ICollection<T> obj) =>
        Format(obj);

    public virtual string FormatCollectionPage<T>(ResourcesPageModel<T> obj) =>
        Format(obj);

    public virtual string Format<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented, Converters);
    }
}

public class ByteValueFormatter
{
    public bool HumanReadable { get; }

    public ByteValueFormatter(bool humanReadable)
    {
        HumanReadable = humanReadable;
    }

    public string Format(long bytes, int decimalPlaces = 1)
    {
        int i = 0;
        decimal dValue = (decimal)bytes;

        if (bytes < 0)
        {
            return "-" + Format(-bytes);
        }

        while (dValue >= 1000 && i < SizeSuffixes.Length)
        {
            dValue /= 1024;
            i++;
        }

        return string.Format("{0,5:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
    }

    private static readonly string[] SizeSuffixes =
          { "B ", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

}
