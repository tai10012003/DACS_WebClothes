using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string? BrandName { get; set; }

    public string? Description { get; set; }

    public int? OrderIndex { get; set; }

    public string? Link { get; set; }

    public int? Hidden { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
