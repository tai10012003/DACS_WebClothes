using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class CartDetail
{
    public int CartDetailId { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int SoldNum { get; set; }

    public int? Hidden { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
