using library.Entities;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace library.Context;

public partial class FanverseDbContext : DbContext
{
    private static FanverseDbContext? _context = new();

    public FanverseDbContext()
    {
    }

    public FanverseDbContext(DbContextOptions<FanverseDbContext> options)
        : base(options)
    {
    }

    public static FanverseDbContext GetContext()
        => _context ??= new FanverseDbContext();    

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Bookmark> Bookmarks { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<ContentWarning> ContentWarnings { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<ModerationRejection> ModerationRejections { get; set; }

    public virtual DbSet<ParentComment> ParentComments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLike> UserLikes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["FanverseDB"].ConnectionString;
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("enum_books_moderationStatus", new[] { "Draft", "Submitted", "Published", "Rejected" })
            .HasPostgresEnum("enum_books_rating", new[] { "General", "Teen", "Mature", "Explicit" })
            .HasPostgresEnum("enum_books_status", new[] { "Draft", "Published", "Archived" })
            .HasPostgresEnum("enum_chapters_moderationStatus", new[] { "Draft", "Submitted", "Published", "Rejected" })
            .HasPostgresEnum("enum_moderation_rejections_status", new[] { "Active", "Solved" })
            .HasPostgresEnum("enum_moderation_rejections_violationType", new[] { "Spam", "HateSpeech", "Copyright", "Explicit", "Other" });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("books_pkey");

            entity.ToTable("books");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CoverUrl).HasColumnName("coverUrl");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false)
                .HasColumnName("isCompleted");
            entity.Property(e => e.Language)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("language");
            entity.Property(e => e.Likes)
                .HasDefaultValue(0)
                .HasColumnName("likes");
            entity.Property(e => e.PublishedAt).HasColumnName("publishedAt");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Views)
                .HasDefaultValue(0)
                .HasColumnName("views");
            entity.Property(e => e.WordCount)
                .HasDefaultValue(0)
                .HasColumnName("wordCount");

            entity.HasOne(d => d.User).WithMany(p => p.Books)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("books_userId_fkey");

            entity.HasMany(d => d.ContentWarnings).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookContentWarning",
                    r => r.HasOne<ContentWarning>().WithMany()
                        .HasForeignKey("ContentWarningId")
                        .HasConstraintName("book_content_warnings_contentWarningId_fkey"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("book_content_warnings_bookId_fkey"),
                    j =>
                    {
                        j.HasKey("BookId", "ContentWarningId").HasName("book_content_warnings_pkey");
                        j.ToTable("book_content_warnings");
                        j.IndexerProperty<int>("BookId").HasColumnName("bookId");
                        j.IndexerProperty<int>("ContentWarningId").HasColumnName("contentWarningId");
                    });

            entity.HasMany(d => d.Genres).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("book_genres_genreId_fkey"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("book_genres_bookId_fkey"),
                    j =>
                    {
                        j.HasKey("BookId", "GenreId").HasName("book_genres_pkey");
                        j.ToTable("book_genres");
                        j.IndexerProperty<int>("BookId").HasColumnName("bookId");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genreId");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("book_tags_tagId_fkey"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("book_tags_bookId_fkey"),
                    j =>
                    {
                        j.HasKey("BookId", "TagId").HasName("book_tags_pkey");
                        j.ToTable("book_tags");
                        j.IndexerProperty<int>("BookId").HasColumnName("bookId");
                        j.IndexerProperty<int>("TagId").HasColumnName("tagId");
                    });
        });

        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("bookmarks_pkey");

            entity.ToTable("bookmarks");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

            entity.HasOne(d => d.Book).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("bookmarks_bookId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("bookmarks_userId_fkey");
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chapters_pkey");

            entity.ToTable("chapters");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.ChapterNumber)
                .HasDefaultValue(1)
                .HasColumnName("chapterNumber");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDraft)
                .HasDefaultValue(true)
                .HasColumnName("isDraft");
            entity.Property(e => e.Likes)
                .HasDefaultValue(0)
                .HasColumnName("likes");
            entity.Property(e => e.PublishedAt).HasColumnName("publishedAt");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
            entity.Property(e => e.Views)
                .HasDefaultValue(0)
                .HasColumnName("views");
            entity.Property(e => e.WordCount)
                .HasDefaultValue(0)
                .HasColumnName("wordCount");

            entity.HasOne(d => d.Book).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("chapters_bookId_fkey");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chats_pkey");

            entity.ToTable("chats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstParticipantId).HasColumnName("firstParticipantId");
            entity.Property(e => e.LastMessageAt).HasColumnName("lastMessageAt");
            entity.Property(e => e.SecondParticipantId).HasColumnName("secondParticipantId");

            entity.HasOne(d => d.FirstParticipant).WithMany(p => p.ChatFirstParticipants)
                .HasForeignKey(d => d.FirstParticipantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("chats_firstParticipantId_fkey");

            entity.HasOne(d => d.SecondParticipant).WithMany(p => p.ChatSecondParticipants)
                .HasForeignKey(d => d.SecondParticipantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("chats_secondParticipantId_fkey");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comments_pkey");

            entity.ToTable("comments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.ChapterId).HasColumnName("chapterId");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Book).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("comments_bookId_fkey");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("comments_chapterId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("comments_userId_fkey");
        });

        modelBuilder.Entity<ContentWarning>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("content_warnings_pkey");

            entity.ToTable("content_warnings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genres_pkey");

            entity.ToTable("genres");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatId).HasColumnName("chatId");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("isRead");
            entity.Property(e => e.SendAt).HasColumnName("sendAt");
            entity.Property(e => e.SenderId).HasColumnName("senderId");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("messages_chatId_fkey");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("messages_senderId_fkey");
        });

        modelBuilder.Entity<ModerationRejection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("moderation_rejections_pkey");

            entity.ToTable("moderation_rejections");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.ChapterId).HasColumnName("chapterId");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Book).WithMany(p => p.ModerationRejections)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("moderation_rejections_bookId_fkey");

            entity.HasOne(d => d.Chapter).WithMany(p => p.ModerationRejections)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("moderation_rejections_chapterId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ModerationRejections)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("moderation_rejections_userId_fkey");
        });

        modelBuilder.Entity<ParentComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parent_comments_pkey");

            entity.ToTable("parent_comments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommentId).HasColumnName("commentId");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Comment).WithMany(p => p.ParentComments)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("parent_comments_commentId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ParentComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("parent_comments_userId_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tags_pkey");

            entity.ToTable("tags");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Nickname, "users_nickname_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatarUrl");
            entity.Property(e => e.BanReason).HasColumnName("banReason");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.HashedPassword)
                .HasMaxLength(255)
                .HasColumnName("hashedPassword");
            entity.Property(e => e.IsBanned)
                .HasDefaultValue(false)
                .HasColumnName("isBanned");
            entity.Property(e => e.Nickname)
                .HasMaxLength(255)
                .HasColumnName("nickname");
            entity.Property(e => e.RegistrationDate).HasColumnName("registrationDate");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("user_roles_roleId_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("user_roles_userId_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("user_roles_pkey");
                        j.ToTable("user_roles");
                        j.IndexerProperty<int>("UserId").HasColumnName("userId");
                        j.IndexerProperty<int>("RoleId").HasColumnName("roleId");
                    });
        });

        modelBuilder.Entity<UserLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_likes_pkey");

            entity.ToTable("user_likes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.ChapterId).HasColumnName("chapterId");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Book).WithMany(p => p.UserLikes)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_likes_bookId_fkey");

            entity.HasOne(d => d.Chapter).WithMany(p => p.UserLikes)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_likes_chapterId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_likes_userId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
