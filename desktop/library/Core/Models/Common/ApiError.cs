using System.Text.Json.Serialization;

namespace library.Core.Models.Common
{
    public class ApiError
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("errors")]
        public Dictionary<string, string[]>? Errors { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Message))
                return Message;

            if (Errors != null && Errors.Count != 0)
                return string.Join("\n", Errors.SelectMany(x => x.Value));

            return "Произошла неизвестная ошибка";
        }
    }
}