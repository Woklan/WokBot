using System.Diagnostics;

namespace WokBot.Interfaces
{
    public interface IFfmpegService
    {
        Process CreateFfmpegInstance(string path);
        void CloseFfmpegInstance(Process process);
    }
}
