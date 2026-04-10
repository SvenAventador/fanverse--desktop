namespace library.Entities;

public partial class Chapter
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Content { get; set; } = null!;

    public int? ChapterNumber { get; set; }

    public int? WordCount { get; set; }

    public int? Views { get; set; }

    public int? Likes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool? IsDraft { get; set; }

    public int? BookId { get; set; }

    public virtual Book? Book { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<ModerationRejection> ModerationRejections { get; set; } = [];

    public virtual ICollection<UserLike> UserLikes { get; set; } = [];
}