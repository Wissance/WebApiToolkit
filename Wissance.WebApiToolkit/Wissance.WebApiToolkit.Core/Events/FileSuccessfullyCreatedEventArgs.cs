using System;

namespace Wissance.WebApiToolkit.Core.Events
{
    public class FileSuccessfullyCreatedEventArgs : EventArgs
    {
        public FileSuccessfullyCreatedEventArgs(string storageId, string path)
        {
            StorageId = storageId;
            Path = path;
        }

        public string StorageId { get; set; }
        public string Path { get; set; }
    }
}