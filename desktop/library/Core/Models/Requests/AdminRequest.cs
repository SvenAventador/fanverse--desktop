using System.Text.Json.Serialization;

namespace library.Core.Models.Requests
{
    public class AdminRequest
    {
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("hashedPassword")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("registrationDate")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}