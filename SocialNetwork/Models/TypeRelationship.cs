using System;
using System.Collections.Generic;

namespace SocialNetwork.Models;

public partial class TypeRelationship
{
    public int TypeId { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<Relationship> Relationships { get; } = new List<Relationship>();
}
