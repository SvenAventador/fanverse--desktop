namespace library.Core.Constants
{
    public static class ApiEndpoints
    {
        public const string BaseUrl = "http://localhost:7000/api";

        public static class Auth
        {
            public const string Login = $"{BaseUrl}/auth/login";
            public const string Logout = $"{BaseUrl}/auth/logout";
            public const string Check = $"{BaseUrl}/auth/check";
        }

        public static class Admin
        {
            public const string GetMe = $"{BaseUrl}/admin/me";
            public const string SetAvatar = $"{BaseUrl}/admin/avatar";
            public const string GetAll = $"{BaseUrl}/admin/all";
        }

    }
}