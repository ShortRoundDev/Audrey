using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audrey.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Chapter
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("time_base")]
        public string TimeBase { get; set; }

        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("start_time")]
        public string StartTime { get; set; }

        [JsonPropertyName("end")]
        public int End { get; set; }

        [JsonPropertyName("end_time")]
        public string EndTime { get; set; }

        [JsonPropertyName("tags")]
        public Tags Tags { get; set; }
    }

    public class FfprobeOutput
    {
        [JsonPropertyName("chapters")]
        public List<Chapter> Chapters { get; set; }
    }

    public class Tags
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }

}
