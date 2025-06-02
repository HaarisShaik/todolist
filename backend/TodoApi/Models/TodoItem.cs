using System.Text.Json.Serialization;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }

        [JsonPropertyName("task")]
        public string? Task { get; set; }

        [JsonPropertyName("completed")]
        public bool IsComplete { get; set; }
    }
}
