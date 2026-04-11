using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class BookDetailResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = null!;

        [JsonPropertyName("coverUrl")]
        public string CoverUrl { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = null!;

        [JsonPropertyName("language")]
        public string Language { get; set; } = null!;

        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonPropertyName("moderationStatus")]
        public string ModerationStatus { get; set; } = null!;

        [JsonPropertyName("wordCount")]
        public int WordCount { get; set; }

        [JsonPropertyName("views")]
        public int Views { get; set; }

        [JsonPropertyName("likes")]
        public int Likes { get; set; }

        [JsonPropertyName("totalComments")]
        public int TotalComments { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("publishedAt")]
        public DateTime? PublishedAt { get; set; }

        [JsonPropertyName("user")]
        public UserResponse User { get; set; } = null!;
    }
}