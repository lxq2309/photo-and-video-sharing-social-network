using System;
using System.Collections.Generic;

namespace SocialNetwork.Models;

public partial class Medium
{
    public int MediaId { get; set; }

    public string? MediaType { get; set; }

    public string? MediaLink { get; set; }

    public int? PostId { get; set; }

    public virtual Post? Post { get; set; }
}
