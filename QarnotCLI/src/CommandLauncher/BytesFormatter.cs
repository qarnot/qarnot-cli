namespace QarnotCLI
{
    public interface IFormatter
    {
        string ConvertToHumanReadable(long value, int decimalPlaces = 1);
    }

    public class BytesFormatter : IFormatter
    {
        private static readonly string[] SizeSuffixes =
                  { "B ", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public string ConvertToHumanReadable(long value, int decimalPlaces = 1)
        {
            int i = 0;
            decimal dValue = (decimal)value;

            if (value < 0)
            {
                return "-" + ConvertToHumanReadable(-value);
            }

            while (dValue >= 1000 && i < SizeSuffixes.Length)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0,5:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }
    }
}