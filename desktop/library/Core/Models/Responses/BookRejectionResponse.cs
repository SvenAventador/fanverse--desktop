using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class BookRejectionResponse
    {
        [JsonPropertyName("hasRejection")]
        public bool HasRejection { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = null!;

        [JsonPropertyName("violationType")]
        public string ViolationType { get; set; } = null!;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}