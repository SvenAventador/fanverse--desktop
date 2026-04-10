namespace library.Entities;

public partial class Chat
{
    public int Id { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public int? FirstParticipantId { get; set; }

    public int? SecondParticipantId { get; set; }

    public virtual User? FirstParticipant { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = [];

    public virtual User? SecondParticipant { get; set; }
}