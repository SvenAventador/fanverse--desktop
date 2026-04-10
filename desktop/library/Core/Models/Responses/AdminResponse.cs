using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class AdminResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; } = null!;

        [JsonPropertyName("registrationDate")]
        public DateTime? RegistrationDate { get; set; }
    }
}