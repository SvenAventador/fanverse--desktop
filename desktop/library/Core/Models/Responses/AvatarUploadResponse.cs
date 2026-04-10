using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class AvatarUploadResponse
    {
        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; } = null!;

        [JsonPropertyName("token")]  
        public string? Token { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}