using System;
using System.Collections.Generic;

namespace SocialNetwork.Models;

public partial class Relationship
{
    public DateTime? CreateAt { get; set; }

    public int? SourceAccountId { get; set; }

    public int? TargetAccountId { get; set; }

    public int? TypeId { get; set; }

    public virtual Account? SourceAccount { get; set; }

    public virtual Account? TargetAccount { get; set; }

    public virtual TypeRelationship? Type { get; set; }
}
