using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class CommentBlog
{
    public int ComBlogId { get; set; }

    public int BlogId { get; set; }

    public int UserId { get; set; }

    public string? ComBlogName { get; set; }

    public DateTime? DateBegin { get; set; }

    public int? OrderIndex { get; set; }

    public int? Hidden { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
