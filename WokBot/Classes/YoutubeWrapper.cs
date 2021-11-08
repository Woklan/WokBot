using FFmpeg.NET;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WokBot.Classes
{
    public class YoutubeWrapper
    {
        private string _format;
        private string _paths;
        private string _output;
        private string _input;
        private string _formattedOutput;
        private bool _fiveSecondFlag;

        public YoutubeWrapper(string input, bool fiveSecondFlag = false)
        {
            _format = "mp3";
            _paths = Program.resourcesInterface.video_output;
            _output = System.Guid.NewGuid().ToString();
            _input = input;
            _fiveSecondFlag = fiveSecondFlag;
            _formattedOutput = _output + "." + _format;
        }

        ~YoutubeWrapper()
        {
            if (File.Exists(Program.resourcesInterface.video_output + _formattedOutput)) File.Delete(Program.resourcesInterface.video_output + _formattedOutput);
        }

        // Cuts the inputted youtube video, and updates relevant values
        // INPUTS   :
        // OUTPUTS  :
        private async Task CutVideo()
        {
            // Sets up input for FFMPEG
            InputFile inputFile = new InputFile(Program.resourcesInterface.video_output + _output + "." + _format);
            string outputFilename = System.Guid.NewGuid().ToString() + ".mp3";
            OutputFile outputFile = new OutputFile(Program.resourcesInterface.video_output + outputFilename);
            Engine ffmpeg = new Engine(Program.resourcesInterface.ffmpeg_executable);

            // Sets up options for FFMPEG
            ConversionOptions options = new();
            options.CutMedia(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5));

            // Cuts video down to 5 seconds long
            await ffmpeg.ConvertAsync(inputFile, outputFile, options);

            // Deletes the original video, and replaces with the new 5 second video
            this.delete();
            _output = outputFilename;
            _formattedOutput = outputFilename;
        }

  
        // Downloads the inputted youtube video, and returns the filename
        // INPUTS   :
        // OUTPUTS  : STRING | filename
        public async Task<string> download()
        {
            // Sets up relevant arguments for downloading the video
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow  = true,
                UseShellExecute = false,
                FileName        = Program.resourcesInterface.video_executable,
                WindowStyle     = ProcessWindowStyle.Hidden,
                
                Arguments       =
                    "--no-playlist "    +
                    "--extract-audio "  +
                    "--audio-format "   +   "mp3 "  +
                    "--audio-quality "  +   "0 "    +
                    "--format "         +   "mp4 "  +
                    "--paths "          +   _paths  +   " "         +
                    "--output "         +   _output +   ".%(ext)s"  +
                    " "                 +   _input
            };

            

            // Runs the Process, and waits for it to complete
            using(Process exeProcess = Process.Start(startInfo))
            {
                await exeProcess.WaitForExitAsync().ConfigureAwait(false);
            }

            // If there is a five second flag, cut the video down to five seconds
            if (_fiveSecondFlag) await this.CutVideo();

            return _formattedOutput;
        }

        // Handles deleting any created files, if they exist.
        // INPUT    : 
        // OUTPUT   :
        public void delete()
        {
            if (File.Exists(Program.resourcesInterface.video_output + _formattedOutput)) File.Delete(Program.resourcesInterface.video_output + _formattedOutput);
            _output = null;
            _formattedOutput = null;
        }
    }
}
