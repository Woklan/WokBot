using System.Diagnostics;

namespace WokBot.Services
{
    public class FfmpegService
    {
        public Process CreateFfmpegInstance(string path)
            => Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });

        public void CloseFfmpegInstance(Process process)
            => process.Close();
    }
}
