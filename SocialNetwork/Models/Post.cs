using System;
using System.Collections.Generic;

namespace SocialNetwork.Models;

public partial class Post
{
    public int PostId { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? LikeCount { get; set; }

    public int? CommentCount { get; set; }

    public string? Content { get; set; }

    public bool? IsDeleted { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Medium> Media { get; } = new List<Medium>();

    public virtual ICollection<Notification> Notifications { get; } = new List<Notification>();

    public virtual ICollection<Account> Accounts { get; } = new List<Account>();
}
