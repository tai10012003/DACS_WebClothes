using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Detail { get; set; }

    public string? Image { get; set; }

    public DateTime? DateBegin { get; set; }

    public int? UserId { get; set; }

    public int? OrderIndex { get; set; }

    public string? Link { get; set; }

    public int? Hidden { get; set; }

    public virtual ICollection<CommentBlog> CommentBlogs { get; set; } = new List<CommentBlog>();

    public virtual User? User { get; set; }
}
