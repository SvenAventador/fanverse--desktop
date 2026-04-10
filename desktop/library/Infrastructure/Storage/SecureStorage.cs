using library.Core.Constants;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace library.Infrastructure.Storage
{
    public static class SecureStorage
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("FanVerse_Salt_2024");

        public static void SaveToken(string token)
        {
            try
            {
                var encryptedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(token), 
                                                          Entropy, 
                                                          DataProtectionScope.CurrentUser);
                using var key = Registry.CurrentUser
                                        .CreateSubKey(Consts.RegistryPath);
                key.SetValue("Token", Convert.ToBase64String(encryptedData));
                key.SetValue("TokenExpiry", DateTime.UtcNow
                                                    .AddDays(7)
                                                    .ToString("o"));
            }
            catch { }
        }

        public static string GetToken()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(Consts.RegistryPath);
                var encryptedBase64 = key?.GetValue("Token")?.ToString();
                var expiryStr = key?.GetValue("TokenExpiry")?.ToString();

                if (string.IsNullOrEmpty(encryptedBase64))
                    return null!;

                if (!string.IsNullOrEmpty(expiryStr) && 
                    DateTime.TryParse(expiryStr, out var expiry))
                {
                    if (expiry < DateTime.UtcNow)
                    {
                        ClearToken();
                        return null!;
                    }
                }

                var encryptedData = Convert.FromBase64String(encryptedBase64);
                var decryptedData = ProtectedData.Unprotect(encryptedData, 
                                                            Entropy, 
                                                            DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return null!;
            }
        }

        public static void ClearToken()
        {
            try
            {
                using var key = Registry.CurrentUser
                                        .CreateSubKey(Consts.RegistryPath);
                key.DeleteValue("Token", false);
                key.DeleteValue("TokenExpiry", false);
            }
            catch { }
        }

        public static void SaveCredentials(string email, bool remember)
        {
            try
            {
                using var key = Registry.CurrentUser
                                        .CreateSubKey(Consts.RegistryPath);
                if (remember && 
                    !string.IsNullOrEmpty(email))
                {
                    key.SetValue("Email", email);
                    key.SetValue("Remember", true);
                }
                else
                {
                    key.DeleteValue("Email", false);
                    key.SetValue("Remember", false);
                }
            }
            catch { }
        }

        public static (string Email, bool Remember) LoadCredentials()
        {
            try
            {
                using var key = Registry.CurrentUser
                                        .OpenSubKey(Consts.RegistryPath);
                var email = key?.GetValue("Email")?.ToString();
                var remember = key?.GetValue("Remember")?.ToString() == "True";
                return (email, remember)!;
            }
            catch
            {
                return (null, false);
            }
        }
    }
}