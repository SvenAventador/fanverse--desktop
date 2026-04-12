using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace library.Core.Models.Responses
{
    public class StatisticsResponse
    {
        [JsonPropertyName("tags")]
        public TagsStats? Tags { get; set; }

        [JsonPropertyName("genres")]
        public GenresStats? Genres { get; set; }

        [JsonPropertyName("contentWarnings")]
        public ContentWarningsStats? ContentWarnings { get; set; }

        [JsonPropertyName("users")]
        public UsersStats? Users { get; set; }

        [JsonPropertyName("books")]
        public BooksStats? Books { get; set; }

        [JsonPropertyName("chapters")]
        public ChaptersStats? Chapters { get; set; }

        [JsonPropertyName("engagement")]
        public EngagementStats? Engagement { get; set; }

        [JsonPropertyName("topAuthors")]
        public List<TopAuthor>? TopAuthors { get; set; }

        [JsonPropertyName("topBooks")]
        public List<TopBook>? TopBooks { get; set; }

        [JsonPropertyName("recentActivity")]
        public List<RecentActivity>? RecentActivity { get; set; }

        [JsonPropertyName("moderationQueue")]
        public ModerationQueue? ModerationQueue { get; set; }

        [JsonPropertyName("summary")]
        public SummaryStats? Summary { get; set; }
    }

    public class TagsStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class GenresStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class ContentWarningsStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class UsersStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("banned")]
        public int Banned { get; set; }

        [JsonPropertyName("active")]
        public int Active { get; set; }

        [JsonPropertyName("bannedPercentage")]
        public string? BannedPercentage { get; set; }

        [JsonPropertyName("moderators")]
        public int Moderators { get; set; }
    }

    public class BooksStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("published")]
        public int Published { get; set; }

        [JsonPropertyName("draft")]
        public int Draft { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("rejected")]
        public int Rejected { get; set; }

        [JsonPropertyName("pendingPercentage")]
        public double? PendingPercentage { get; set; }

        [JsonPropertyName("byRating")]
        public List<RatingStats>? ByRating { get; set; }

        [JsonPropertyName("byLanguage")]
        public List<LanguageStats>? ByLanguage { get; set; }
    }

    public class RatingStats
    {
        [JsonPropertyName("rating")]
        public string? Rating { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class LanguageStats
    {
        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("count")]
        public string? Count { get; set; }
    }

    public class ChaptersStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("rejected")]
        public int Rejected { get; set; }

        [JsonPropertyName("averagePerBook")]
        public double? AveragePerBook { get; set; }
    }

    public class EngagementStats
    {
        [JsonPropertyName("totalComments")]
        public int TotalComments { get; set; }

        [JsonPropertyName("totalLikes")]
        public int TotalLikes { get; set; }

        [JsonPropertyName("averageLikesPerBook")]
        public double? AverageLikesPerBook { get; set; }

        [JsonPropertyName("averageCommentsPerBook")]
        public double? AverageCommentsPerBook { get; set; }
    }

    public class TopAuthor
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }

        [JsonPropertyName("avatarUrl")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("bookCount")]
        public int BookCount { get; set; }

        [JsonPropertyName("totalViews")]
        public int TotalViews { get; set; }

        [JsonPropertyName("totalLikes")]
        public int TotalLikes { get; set; }
    }

    public class TopBook
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("views")]
        public int Views { get; set; }

        [JsonPropertyName("likes")]
        public int Likes { get; set; }

        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }
    }

    public class RecentActivity
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }
    }

    public class ModerationQueue
    {
        [JsonPropertyName("books")]
        public List<object>? Books { get; set; }

        [JsonPropertyName("chapters")]
        public List<object>? Chapters { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class SummaryStats
    {
        [JsonPropertyName("totalContent")]
        public int TotalContent { get; set; }

        [JsonPropertyName("moderationPending")]
        public int ModerationPending { get; set; }

        [JsonPropertyName("moderationSuccessRate")]
        public double? ModerationSuccessRate { get; set; }
    }
}