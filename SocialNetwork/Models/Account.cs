using System;
using System.Collections.Generic;

namespace SocialNetwork.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string? FullName { get; set; }

    public string? DisplayName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? AboutMe { get; set; }

    public string? Location { get; set; }

    public int? NumberNoti { get; set; }

    public string? Phone { get; set; }

    public int? Follower { get; set; }

    public int? Following { get; set; }

    public bool? IsActive { get; set; }

    public string? AccountType { get; set; }

    public bool? IsAdmin { get; set; }

    public string? Avatar { get; set; }

    public DateTime? DayOfBirth { get; set; }

    public bool? Gender { get; set; }

    public bool? IsBanned { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; } = new List<Notification>();

    public virtual ICollection<Post> PostsNavigation { get; } = new List<Post>();

    public virtual ICollection<ChatSession> Chats { get; } = new List<ChatSession>();

    public virtual ICollection<Post> Posts { get; } = new List<Post>();
}
