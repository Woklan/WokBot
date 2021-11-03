using System.Diagnostics;
using System.IO;

namespace WokBot.Classes
{
    class YoutubeWrapper
    {
        private string _format;
        private string _paths;
        private string _output;
        private string _input;

        public YoutubeWrapper(string input)
        {
            _format = "mp4";
            _paths = Program.resourcesInterface.video_output;
            _output = System.Guid.NewGuid().ToString();
            _input = input;
        }

        public void download()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            startInfo.FileName = "yt-dlp.exe";

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            startInfo.Arguments = "--format " + _format + " --paths " + _paths + " -o " + _output + " " + _input;

            using(Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }
        }

        public void delete()
        {
            if (File.Exists(Program.resourcesInterface.video_output + _output)) File.Delete(Program.resourcesInterface.video_output + _output);
        }
    }
}
