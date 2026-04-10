namespace library.Entities;

public partial class ModerationRejection
{
    public int Id { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? BookId { get; set; }

    public int? ChapterId { get; set; }

    public int? UserId { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual User? User { get; set; }
}