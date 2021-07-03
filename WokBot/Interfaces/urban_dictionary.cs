using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WokBot.Interfaces
{
    public class urban_data
    {
        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("thumbs_up")]
        public int ThumbsUp { get; set; }

        [JsonProperty("sound_urls")]
        public List<object> SoundUrls { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("defid")]
        public int Defid { get; set; }

        [JsonProperty("current_vote")]
        public string CurrentVote { get; set; }

        [JsonProperty("written_on")]
        public DateTime WrittenOn { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }

        [JsonProperty("thumbs_down")]
        public int ThumbsDown { get; set; }
    }
    class urban_dictionary
    {
        [JsonProperty("list")]
        public List<urban_data> data { get; set; }
    }
}
