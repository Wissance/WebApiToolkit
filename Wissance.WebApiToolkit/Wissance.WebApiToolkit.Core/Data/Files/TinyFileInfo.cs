namespace Wissance.WebApiToolkit.Core.Data.Files
{
    public class TinyFileInfo
    {
        public TinyFileInfo()
        {
        }

        public TinyFileInfo(string name, string path, bool isDirectory, long size)
        {
            Name = name;
            Path = path;
            IsDirectory = isDirectory;
            Size = size;
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
    }
}