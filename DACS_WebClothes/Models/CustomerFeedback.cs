using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class CustomerFeedback
{
    public int CusFeedbackId { get; set; }

    public int UserId { get; set; }

    public string? CusFeedbackName { get; set; }

    public DateTime? DateBegin { get; set; }

    public int? OrderIndex { get; set; }

    public int? Hidden { get; set; }

    public virtual User User { get; set; } = null!;
}
