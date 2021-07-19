using Discord;
using Discord.Audio;
using FFmpeg.NET;
using Newtonsoft.Json;
using NYoutubeDL;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WokBot.Classes
{
    public class Utility
    {
        static readonly HttpClient client = new HttpClient();
        private static Random _random = new Random();
        private YoutubeDL _youtubeDL = new YoutubeDL();
        private List<ulong> _currentVoiceChat = new List<ulong>();

        public Utility()
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            _youtubeDL.Options.FilesystemOptions.Output = Program.resourcesInterface.video_output + "video.mp3";
            _youtubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            _youtubeDL.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;
            _youtubeDL.YoutubeDlPath = Program.resourcesInterface.video_executable;
            Console.WriteLine(Program.resourcesInterface.video_executable);
            if (File.Exists("youtube-dl"))
            {
                Console.WriteLine("IT EXISTS");
            }
            _youtubeDL.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            _youtubeDL.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);
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

        // Generates a random string
        public string GenerateRandomString(int num)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz_";
            return new string(Enumerable.Repeat(chars, num).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public async Task YoutubeDownloadID(string video_id, string file_name = "video.mp3")
        {
            _youtubeDL.Options.FilesystemOptions.Output = Program.resourcesInterface.video_output + file_name;
            _youtubeDL.VideoUrl = "https://www.youtube.com/watch?v=" + video_id;
            await _youtubeDL.DownloadAsync();
        }

        public async Task YoutubeDownloadLink(string video_link, string file_name = "video.mp3")
        {
            _youtubeDL.Options.FilesystemOptions.Output = Program.resourcesInterface.video_output + file_name;
            _youtubeDL.VideoUrl = video_link;
            await _youtubeDL.DownloadAsync();
        }

        // Creates a new Process dedicated for FFMPEG (Voice Chat)
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
            // Bot joins the voice channel
            var audioClient = await channel.ConnectAsync();
            Console.WriteLine("ALERT: Connected to voice!");

            // Bots plays the video in the voice channel
            using (var ffmpeg = Program.utility.CreateStream(Program.resourcesInterface.video_output + "video2.mp3"))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }

            // Bot Disconnects from Voice
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

        public async Task<MediaFile> CutVideo(string input_file, string output_file, int cut_begin, int cut_end)
        {
            var input = new InputFile(Program.resourcesInterface.video_output + input_file);
            var output = new OutputFile(Program.resourcesInterface.video_output + output_file);
            var ffmpeg = new Engine(Program.resourcesInterface.ffmpeg_executable);

            var options = new ConversionOptions();

            options.CutMedia(TimeSpan.FromSeconds(cut_begin), TimeSpan.FromSeconds(cut_end));
            var ffmpeg_output = await ffmpeg.ConvertAsync(input, output, options);
            return ffmpeg_output;
        }

        public async Task<T> UploadAsync<T>(string url, string filename, Stream file, List<(string, string)> param)
        {
            HttpContent fileContent = new StreamContent(file);

            var formData = new MultipartFormDataContent();

            for(int i = 0; i < param.Count; i++)
            {
                HttpContent stringContent = new StringContent(param.ElementAt(i).Item2);
                Console.WriteLine(stringContent.ReadAsStringAsync() + " | " + param.ElementAt(i).Item1 + " | " + param.ElementAt(i).Item2);
                formData.Add(stringContent, param.ElementAt(i).Item1);
                
            }

            formData.Add(fileContent, "file", filename);

            var response = await client.PostAsync(url, formData);

            string parse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(parse);
        }
        public string audDError(string errorNum)
        {
            switch (errorNum)
            {
                case "901":
                    return "No Api Token Passed";
                    break;
                case "900":
                    return "Wrong Api Token";
                    break;
                case "600":
                    return "Incorrect URL";
                    break;
                case "700":
                    return "Didn't recieve the file";
                    break;
                case "500":
                    return "Incorrect audio file";
                    break;
                case "400":
                    return "Audio file too big";
                    break;
                case "300":
                    return "Fingerprinting Error (Audio file too small)";
                    break;
                case "100":
                    return "Unknown Error";
                    break;
                default:
                    return "This is a known, but not accounted for, error";
            }
        }
    }

    
}
