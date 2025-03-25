using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public string? OrderId { get; set; }

    public DateTime DateBegin { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public int? Hidden { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Payment Payments { get; set; } = null;

    public virtual User User { get; set; } = null!;
}
