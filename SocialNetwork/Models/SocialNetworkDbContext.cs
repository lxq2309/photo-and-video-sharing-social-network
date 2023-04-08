using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Models;

public partial class SocialNetworkDbContext : DbContext
{
    public SocialNetworkDbContext()
    {
    }

    public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<ChatSession> ChatSessions { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Relationship> Relationships { get; set; }

    public virtual DbSet<TypeRelationship> TypeRelationships { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-DCA8K2S\\MSSQLSERVER01;Initial Catalog=SocialNetworkDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA58621A32D7D");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.AboutMe).HasColumnType("ntext");
            entity.Property(e => e.AccountType)
                .HasMaxLength(10)
                .HasDefaultValueSql("('Public')");
            entity.Property(e => e.Avatar)
                .HasMaxLength(100)
                .HasDefaultValueSql("('default.jpg')");
            entity.Property(e => e.DayOfBirth).HasColumnType("date");
            entity.Property(e => e.DisplayName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Follower).HasDefaultValueSql("((0))");
            entity.Property(e => e.Following).HasDefaultValueSql("((0))");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isAdmin");
            entity.Property(e => e.IsBanned)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isBanned");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasMany(d => d.Chats).WithMany(p => p.Accounts)
                .UsingEntity<Dictionary<string, object>>(
                    "AccountHasChatSession",
                    r => r.HasOne<ChatSession>().WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AccountHa__ChatI__59FA5E80"),
                    l => l.HasOne<Account>().WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AccountHa__Accou__59063A47"),
                    j =>
                    {
                        j.HasKey("AccountId", "ChatId").HasName("PK__AccountH__4E021BE48334DD33");
                        j.ToTable("AccountHasChatSession");
                        j.IndexerProperty<int>("AccountId").HasColumnName("AccountID");
                        j.IndexerProperty<int>("ChatId").HasColumnName("ChatID");
                    });

            entity.HasMany(d => d.Posts).WithMany(p => p.Accounts)
                .UsingEntity<Dictionary<string, object>>(
                    "Like",
                    r => r.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Like__PostID__6477ECF3"),
                    l => l.HasOne<Account>().WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Like__AccountID__6383C8BA"),
                    j =>
                    {
                        j.HasKey("AccountId", "PostId").HasName("PK__Like__AE3C838500718F32");
                        j.ToTable("Like");
                        j.IndexerProperty<int>("AccountId").HasColumnName("AccountID");
                        j.IndexerProperty<int>("PostId").HasColumnName("PostID");
                    });
        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__ChatSess__A9FBE62617E953C6");

            entity.ToTable("ChatSession");

            entity.Property(e => e.ChatId).HasColumnName("ChatID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFAA66C569AC");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Content).HasColumnType("ntext");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostId).HasColumnName("PostID");

            entity.HasOne(d => d.Account).WithMany(p => p.Comments)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Comment__Account__4BAC3F29");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Comment__PostID__4CA06362");
        });

        modelBuilder.Entity<Medium>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("PK__Media__B2C2B5AFE1AD5323");

            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.MediaLink).HasMaxLength(100);
            entity.Property(e => e.MediaType).HasMaxLength(10);
            entity.Property(e => e.PostId).HasColumnName("PostID");

            entity.HasOne(d => d.Post).WithMany(p => p.Media)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Media__PostID__4F7CD00D");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__C87C037CFEC5D18B");

            entity.ToTable("Message");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.ChatId).HasColumnName("ChatID");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MessageContent).HasMaxLength(1000);

            entity.HasOne(d => d.Account).WithMany(p => p.Messages)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Message__Account__5535A963");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("FK__Message__ChatID__5629CD9C");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotiId).HasName("PK__Notifica__EDC08EF21A2A34AF");

            entity.ToTable("Notification");

            entity.Property(e => e.NotiId).HasColumnName("NotiID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Content).HasColumnType("ntext");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostId).HasColumnName("PostID");

            entity.HasOne(d => d.Account).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Notificat__Accou__46E78A0C");

            entity.HasOne(d => d.Post).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Notificat__PostI__47DBAE45");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Post__AA12603803139562");

            entity.ToTable("Post");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CommentCount).HasDefaultValueSql("((0))");
            entity.Property(e => e.Content).HasColumnType("ntext");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isDeleted");
            entity.Property(e => e.LikeCount).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Account).WithMany(p => p.PostsNavigation)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Post__AccountID__4316F928");
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Relationship");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SourceAccountId).HasColumnName("SourceAccountID");
            entity.Property(e => e.TargetAccountId).HasColumnName("TargetAccountID");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.SourceAccount).WithMany()
                .HasForeignKey(d => d.SourceAccountId)
                .HasConstraintName("FK__Relations__Sourc__5EBF139D");

            entity.HasOne(d => d.TargetAccount).WithMany()
                .HasForeignKey(d => d.TargetAccountId)
                .HasConstraintName("FK__Relations__Targe__5FB337D6");

            entity.HasOne(d => d.Type).WithMany()
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK__Relations__TypeI__60A75C0F");
        });

        modelBuilder.Entity<TypeRelationship>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__TypeRela__516F03955C76723B");

            entity.ToTable("TypeRelationship");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.TypeName).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
