using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string? DiscountName { get; set; }

    public decimal? DiscountPercent { get; set; }

    public decimal? DiscountNum { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? ProductId { get; set; }

    public int? OrderIndex { get; set; }

    public int? Hidden { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Product? Product { get; set; }
}
