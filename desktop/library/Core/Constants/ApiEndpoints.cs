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

    }
}