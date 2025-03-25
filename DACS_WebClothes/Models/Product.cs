using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public string? Image1 { get; set; }

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public int? OrderIndex { get; set; }

    public string? Link { get; set; }

    public int? Hidden { get; set; }

    public int? CategoryId { get; set; }

    public int? BrandId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<CommentProduct> CommentProducts { get; set; } = new List<CommentProduct>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
