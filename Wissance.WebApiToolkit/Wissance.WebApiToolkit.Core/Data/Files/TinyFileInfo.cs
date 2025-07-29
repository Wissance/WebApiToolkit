namespace Wissance.WebApiToolkit.Core.Data.Files
{
    public class TinyFileInfo
    {
        public TinyFileInfo()
        {
        }

        public TinyFileInfo(string name, bool isDirectory, long size)
        {
            Name = name;
            IsDirectory = isDirectory;
            Size = size;
        }

        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
    }
}