using Newtonsoft.Json;

namespace WokBot.Interfaces
{
    public class ResourcesInterface
    {
        [JsonProperty("discord")]
        public string discord { get; set; }
        [JsonProperty("virus_total")]
        public string virus_total { get; set; }
        [JsonProperty("youtube")]
        public string youtube { get; set; }
        [JsonProperty("video_executable")]
        public string video_executable { get; set; }
        [JsonProperty("video_output")]
        public string video_output { get; set; }
        [JsonProperty("ffmpeg_executable")]
        public string ffmpeg_executable { get; set; }
        [JsonProperty("docker")]
        public string docker { get; set; }
        [JsonProperty("audD")]
        public string audD { get; set; }
    }
}
