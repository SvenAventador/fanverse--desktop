namespace library.Entities;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Summary { get; set; }

    public string? CoverUrl { get; set; }

    public string Language { get; set; } = null!;

    public bool? IsCompleted { get; set; }

    public int? WordCount { get; set; }

    public int? Views { get; set; }

    public int? Likes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = [];

    public virtual ICollection<Chapter> Chapters { get; set; } = [];

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<ModerationRejection> ModerationRejections { get; set; } = [];

    public virtual User? User { get; set; }

    public virtual ICollection<UserLike> UserLikes { get; set; } = [];

    public virtual ICollection<ContentWarning> ContentWarnings { get; set; } = [];

    public virtual ICollection<Genre> Genres { get; set; } = [];

    public virtual ICollection<Tag> Tags { get; set; } = [];
}