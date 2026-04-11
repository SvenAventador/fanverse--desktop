using System.Text.Json.Serialization;

namespace library.Core.Models.Requests
{
    public class ModerationRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("rejectionReason")]
        public string? RejectionReason { get; set; }

        [JsonPropertyName("violationType")]
        public string? ViolationType { get; set; }
    }
}