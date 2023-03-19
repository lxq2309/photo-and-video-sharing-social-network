using System;
using System.Collections.Generic;

namespace SocialNetwork.Models;

public partial class ChatSession
{
    public int ChatId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<Account> Accounts { get; } = new List<Account>();
}
