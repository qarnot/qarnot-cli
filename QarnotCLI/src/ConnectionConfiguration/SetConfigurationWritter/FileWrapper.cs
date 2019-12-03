namespace QarnotCLI
{
    using System.IO;

    public interface IFileWrapper
    {
        FileStream CreateFile(string path);

        void DeleteFile(string path);

        bool DoesFileExist(string path);

        void CreateDirectory(string path);

        bool DirectoryExist(string path);

        string GetDirectoryName(string path);
    }

    public class FileWrapper : IFileWrapper
    {
        public FileStream CreateFile(string path)
        {
            return File.Create(path);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public bool DoesFileExist(string path)
        {
            return File.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool DirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public string GetDirectoryName(string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }
    }
}
