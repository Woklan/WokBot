using Discord;
using Discord.Audio;
using Newtonsoft.Json;
using NYoutubeDL;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WokBot.Classes
{
    public class Utility
    {
        static readonly HttpClient client = new HttpClient();
        private static Random _random = new Random();
        private YoutubeDL youtubeDL = new YoutubeDL();

        public Utility()
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            youtubeDL.Options.FilesystemOptions.Output = Program.resourcesInterface.video_output + "video.mp3";
            youtubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDL.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;
            youtubeDL.YoutubeDlPath = Program.resourcesInterface.video_executable;
            youtubeDL.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            youtubeDL.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);
        }

        public async Task<T> ApiCall<T>(string url)
        {
            // Get Request to Website
            var data = await client.GetAsync(url);
            
            // Parses content into string
            string parse = await data.Content.ReadAsStringAsync();

            // Returns object created from JSON
            return JsonConvert.DeserializeObject<T>(parse);          
        }

        public string GenerateRandomString(int num)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz_";
            return new string(Enumerable.Repeat(chars, num).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public async Task YoutubeDownload(string video_id)
        {
            youtubeDL.VideoUrl = "https://www.youtube.com/watch?v=" + video_id;
            await youtubeDL.DownloadAsync();
        }

        public Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = Program.resourcesInterface.ffmpeg_executable,
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        public async Task PlayAudio(IVoiceChannel channel)
        {
            var audioClient = await channel.ConnectAsync();
            Console.WriteLine("ALERT: Connected to voice!");

            using (var ffmpeg = Program.utility.CreateStream(Program.resourcesInterface.video_output + "video2.mp3"))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }

            await channel.DisconnectAsync();
            Console.WriteLine("ALERT: Disconnected to voice!");
        }
    }
}
