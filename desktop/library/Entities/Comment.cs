namespace library.Entities;

public partial class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public int? ChapterId { get; set; }

    public int? BookId { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual ICollection<ParentComment> ParentComments { get; set; } = [];

    public virtual User? User { get; set; }
}