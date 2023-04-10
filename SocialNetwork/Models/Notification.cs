using System;
using System.Collections.Generic;

namespace SocialNetwork.Models;

public partial class Notification
{
    public int NotiId { get; set; }

    public int? PostId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? TypeNotification { get; set; }

    public bool? IsRead { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Post? Post { get; set; }
}
