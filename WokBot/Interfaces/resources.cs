using Newtonsoft.Json;

namespace WokBot.Interfaces
{
    class ResourcesInterface
    {
        [JsonProperty("discord")]
        public string discord { get; set; }
    }
}
