namespace QarnotCLI
{
    using System;
    using System.IO;
    using System.Text;

    public interface IFileStreamWriter
    {
        void Write(string prefix, string value, FileStream fs);
    }

    public class FileStreamWriter : IFileStreamWriter
    {
        public void Write(string prefix, string value, FileStream fs)
        {
            byte[] stringToWrite = new UTF8Encoding(true).GetBytes(prefix + "=" + value + Environment.NewLine);
            fs.Write(stringToWrite, 0, stringToWrite.Length);
        }
    }
}
