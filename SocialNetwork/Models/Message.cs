using System;
using System.Collections.Generic;

namespace SocialNetwork.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public string? MessageContent { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? AccountId { get; set; }

    public int? ChatId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ChatSession? Chat { get; set; }
}
