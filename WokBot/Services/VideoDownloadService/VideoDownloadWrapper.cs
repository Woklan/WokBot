using System;
using System.IO;

namespace WokBot.Services.VideoDownloadService
{
    public class VideoDownloadWrapper : IDisposable
    {
        public string FilePath { get; }

        public VideoDownloadWrapper(string filePath)
        {
            FilePath = filePath;
        }

        public void Dispose()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
