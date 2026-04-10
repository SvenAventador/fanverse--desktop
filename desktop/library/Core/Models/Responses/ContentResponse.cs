using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class ContentResponse
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}