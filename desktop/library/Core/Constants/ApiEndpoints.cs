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
            public const string AddNewAdmin = $"{BaseUrl}/admin/create";
            public const string UpdateAdmin = $"{BaseUrl}/admin/update";
            public const string DeleteAdmin = $"{BaseUrl}/admin";
            public const string GetStats = $"{BaseUrl}/admin/stats";
        }

        public static class Tags
        {
            public const string GetAll = $"{BaseUrl}/tag";
            public const string Create = $"{BaseUrl}/tag";
            public const string Update = $"{BaseUrl}/tag";
            public const string Delete = $"{BaseUrl}/tag";
        }

        public static class Genre
        {
            public const string GetAll = $"{BaseUrl}/genre";
            public const string Create = $"{BaseUrl}/genre";
            public const string Update = $"{BaseUrl}/genre";
            public const string Delete = $"{BaseUrl}/genre";
        }

        public static class ContentWarning
        {
            public const string GetAll = $"{BaseUrl}/contentWarning";
            public const string Create = $"{BaseUrl}/contentWarning";
            public const string Update = $"{BaseUrl}/contentWarning";
            public const string Delete = $"{BaseUrl}/contentWarning";
        }

        public static class Moderation
        {
            public const string GetAll = $"{BaseUrl}/admin/all/users";
            public const string NewBanStatus = $"{BaseUrl}/admin/newBanStatus";
        }

        public static class Books
        {
            public const string GetAll = $"{BaseUrl}/admin/all/books";
            public const string GetById = $"{BaseUrl}/admin/books";
            public const string Moderate = $"{BaseUrl}/admin/moderateBook";
            public static string GetChapters(int bookId) => $"{BaseUrl}/admin/books/{bookId}/chapters";
        }

        public static class Chapters
        {
            public const string GetById = $"{BaseUrl}/admin/chapters";
            public const string Moderate = $"{BaseUrl}/admin/moderateChapter";
        }

        public static class Password
        {
            public const string ForgotPassword = $"{BaseUrl}/user/forgot";
            public const string ResetPassword = $"{BaseUrl}/user/reset";
        }
    }
}