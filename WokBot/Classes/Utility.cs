using Discord;
using Discord.Audio;
using Newtonsoft.Json;
using NYoutubeDL;
using System;
using System.Collections.Generic;
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
        private List<ulong> _currentVoiceChat = new List<ulong>();

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
            await removeCurrentVoice(channel);
            Console.WriteLine("ALERT: Disconnected to voice!");
        }

        public async Task<bool> CheckBotInVoiceChat(IVoiceChannel channel)
        {
            // Grabs iterator for Async User List
            IAsyncEnumerator<IReadOnlyCollection<IGuildUser>> async_iterator = channel.GetUsersAsync().GetAsyncEnumerator();

            // Moves iterator
            await async_iterator.MoveNextAsync();

            // Grabs iterator for User List
            IEnumerator<IGuildUser> iterator = async_iterator.Current.GetEnumerator();

            // Moves iterator
            iterator.MoveNext();

            // Loops through User in List
            while (iterator.Current.Id.CompareTo(Program.bot_id) != 0 && iterator.MoveNext()) { };

            bool in_voice_chat = !iterator.MoveNext();
            bool joining_voice_chat = false;

            // Checks if bot is in process of joining
            for(int i = 0; i < _currentVoiceChat.Count; i++)
            {
                if(channel.Id == _currentVoiceChat.ElementAt(i))
                {
                    joining_voice_chat = true;
                }
            }

            // Checks if bot was already in or in the process of joining
            if (in_voice_chat && !joining_voice_chat)
            {
                _currentVoiceChat.Add(channel.Id);
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task removeCurrentVoice(IVoiceChannel channel)
        {
            _currentVoiceChat.Remove(channel.Id);
        }
    }
}
