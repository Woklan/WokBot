using Discord;
using Discord.Audio;
using FFmpeg.NET;
using Microsoft.Extensions.Logging;
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
        private List<ulong> _currentVoiceChat = new List<ulong>();

        public Utility()
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            Console.WriteLine(Program.resourcesInterface.video_executable);
        }

        public async Task<T> ApiCall<T>(string url)
        {
            // Get Request to Website
            HttpResponseMessage data = await client.GetAsync(url);
            
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

        public async Task PlayAudio(IVoiceChannel channel, string fileName)
        {
            // Bot joins the voice channel
            IAudioClient audioClient = await channel.ConnectAsync();
            Program.logger.LogInformation(Program.generateLogNum(), "Connected to voice!");

            // Bots plays the video in the voice channel
            using (Process ffmpeg = Program.utility.CreateStream(Program.resourcesInterface.video_output + fileName))
            using (Stream output = ffmpeg.StandardOutput.BaseStream)
            using (AudioOutStream discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }

            // Bot Disconnects from Voice
            await channel.DisconnectAsync();
            removeCurrentVoice(channel);
            Program.logger.LogInformation(Program.generateLogNum(), "Disconnected to voice");
        }

        public async Task PlayAudio(IVoiceChannel channel)
        {
            // Bot joins the voice channel
            IAudioClient audioClient = await channel.ConnectAsync();
            Program.logger.LogInformation(Program.generateLogNum(), "Connected to voice!");

            // Bots plays the video in the voice channel
            using (Process ffmpeg = Program.utility.CreateStream(Program.resourcesInterface.video_output + "video2.mp3"))
            using (Stream output = ffmpeg.StandardOutput.BaseStream)
            using (AudioOutStream discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }

            // Bot Disconnects from Voice
            await channel.DisconnectAsync();
            removeCurrentVoice(channel);
            Program.logger.LogInformation(Program.generateLogNum(), "Disconnected to voice");
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

        public void removeCurrentVoice(IVoiceChannel channel)
        {
            _currentVoiceChat.Remove(channel.Id);
        }

        public async Task<T> UploadAsync<T>(string url, string filename, Stream file, List<(string, string)> param)
        {
            HttpContent fileContent = new StreamContent(file);

            MultipartFormDataContent formData = new();

            for(int i = 0; i < param.Count; i++)
            {
                HttpContent stringContent = new StringContent(param.ElementAt(i).Item2);
                Console.WriteLine(stringContent.ReadAsStringAsync() + " | " + param.ElementAt(i).Item1 + " | " + param.ElementAt(i).Item2);
                formData.Add(stringContent, param.ElementAt(i).Item1);
                
            }

            formData.Add(fileContent, "file", filename);

            HttpResponseMessage response = await client.PostAsync(url, formData);

            string parse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(parse);
        }
    }
}
