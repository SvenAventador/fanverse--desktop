using System.Text.Json.Serialization;

namespace library.Core.Models.Requests
{
    public class BanRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("banReason")]
        public string? BanReason { get; set; }
    }
}