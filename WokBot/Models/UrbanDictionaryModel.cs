using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WokBot.Models
{
    public class UrbanDictionaryDefinition
    {
        [JsonPropertyName("definition")]
        public string Definition { get; set; }
        
        [JsonPropertyName("permalink")]
        public string Permalink  { get; set; }
        
        [JsonPropertyName("thumbs_up")]
        public int ThumbsUp { get; set; }
        
        [JsonPropertyName("author")]
        public string Author { get; set; }
        
        [JsonPropertyName("word")]
        public string Word { get; set; }
        
        [JsonPropertyName("defid")]
        public int DefId { get; set; }
        
        [JsonPropertyName("current_vote")]
        public string CurrentVote { get; set; }
        
        [JsonPropertyName("written_on")]
        public DateTime WrittenOn { get; set; }
        
        [JsonPropertyName("example")]
        public string Example { get; set; }
        
        [JsonPropertyName("thumbs_down")]
        public int ThumbsDown { get; set; }
    }

    public class UrbanDictionaryResponse
    {
        [JsonPropertyName("list")]
        public List<UrbanDictionaryDefinition> UrbanDictionaryDefinitions { get; set; }
    }
}