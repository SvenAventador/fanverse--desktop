using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class ChapterResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("chapterNumber")]
        public int ChapterNumber { get; set; }

        [JsonPropertyName("wordCount")]
        public int WordCount { get; set; }

        [JsonPropertyName("views")]
        public int Views { get; set; }

        [JsonPropertyName("likes")]
        public int Likes { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("moderationStatus")]
        public string ModerationStatus { get; set; } = null!;
    }
}