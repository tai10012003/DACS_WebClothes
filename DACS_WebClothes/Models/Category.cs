using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int? OrderIndex { get; set; }

    public string? Link { get; set; }

    public int? Hidden { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
