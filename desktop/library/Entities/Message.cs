namespace library.Entities;

public partial class Message
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public DateTime? SendAt { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ChatId { get; set; }

    public int? SenderId { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual User? Sender { get; set; }
}