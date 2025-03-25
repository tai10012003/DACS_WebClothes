using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? CartId { get; set; }

    public int? DiscountId { get; set; }

    public int? UserId { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransportMethod { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual User? User { get; set; }
}
