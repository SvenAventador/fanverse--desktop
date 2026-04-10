using System.Text.Json.Serialization;

namespace library.Core.Models.Requests
{
    public class AdminRequest
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("hashedPassword")]
        public string? Password { get; set; }

        [JsonPropertyName("registrationDate")]
        public DateTime? RegistrationDate { get; set; } = DateTime.Now;
    }
}