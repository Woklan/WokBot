using Newtonsoft.Json;

namespace WokBot.Interfaces
{
    public class ResourcesInterface
    {
        [JsonProperty("discord")]
        public string discord { get; set; }
        [JsonProperty("virus_total")]
        public string virus_total { get; set; }
    }
}
