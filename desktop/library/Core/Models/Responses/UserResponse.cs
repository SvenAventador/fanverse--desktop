using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace library.Core.Models.Responses
{
    public class UserResponse : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; } = null!;

        [JsonPropertyName("isBanned")]
        public bool IsBanned { get; set; }

        [JsonPropertyName("banReason")]
        public string? BanReason { get; set; }

        [JsonPropertyName("registrationDate")]
        public DateTime? RegistrationDate { get; set; }

        [JsonIgnore]
        public bool HasBanReason { get; set; }

        private BitmapImage? _avatarImage;

        [JsonIgnore]
        public BitmapImage? AvatarImage
        {
            get => _avatarImage;
            set
            {
                _avatarImage = value;
                OnPropertyChanged();
            }
        }
    }
}