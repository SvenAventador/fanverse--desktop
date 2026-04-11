using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class ForgotPasswordResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}