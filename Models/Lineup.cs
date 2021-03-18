using System.Text.Json.Serialization;

namespace Homer.Models
{
    public class Lineup
    {
        [JsonIgnore]
        public string DisplayText => $"{GuideName} ({GuideNumber})";

        [JsonPropertyName("GuideNumber")]
        public string GuideNumber
        {
            get;
            set;
        }
        
        [JsonPropertyName("GuideName")]
        public string GuideName
        {
            get;
            set;
        }
        
        [JsonPropertyName("Tags")]
        public string Tags
        {
            get;
            set;
        }
        
        [JsonPropertyName("URL")]
        public string Url
        {
            get;
            set;
        }
    }
}