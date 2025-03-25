using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Feedback
{
    public int FeedBackId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? DateBegin { get; set; }

    public int? OrderIndex { get; set; }

    public int? Hidden { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
