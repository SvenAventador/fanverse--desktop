namespace library.Entities;

public partial class ParentComment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CommentId { get; set; }

    public int? UserId { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual User? User { get; set; }
}