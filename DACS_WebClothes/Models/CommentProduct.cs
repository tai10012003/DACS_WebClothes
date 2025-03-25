using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class CommentProduct
{
    public int ComProId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string? ComProName { get; set; }

    public DateTime? DateBegin { get; set; }

    public int? OrderIndex { get; set; }

    public int? Hidden { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
