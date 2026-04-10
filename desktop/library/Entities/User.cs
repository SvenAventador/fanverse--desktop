namespace library.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Nickname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashedPassword { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public bool? IsBanned { get; set; }

    public string? BanReason { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = [];

    public virtual ICollection<Book> Books { get; set; } = [];

    public virtual ICollection<Chat> ChatFirstParticipants { get; set; } = [];

    public virtual ICollection<Chat> ChatSecondParticipants { get; set; } = [];

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<Message> Messages { get; set; } = [];

    public virtual ICollection<ModerationRejection> ModerationRejections { get; set; } = [];

    public virtual ICollection<ParentComment> ParentComments { get; set; } = [];

    public virtual ICollection<UserLike> UserLikes { get; set; } = [];

    public virtual ICollection<Role> Roles { get; set; } = [];
}