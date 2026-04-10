using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = null!;
    }
}