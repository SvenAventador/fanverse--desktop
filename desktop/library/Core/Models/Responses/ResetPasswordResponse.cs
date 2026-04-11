using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class ResetPasswordResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}