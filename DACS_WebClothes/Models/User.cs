using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? Permission { get; set; }

    public int? Hidden { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<CommentBlog> CommentBlogs { get; set; } = new List<CommentBlog>();

    public virtual ICollection<CommentProduct> CommentProducts { get; set; } = new List<CommentProduct>();

    public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; } = new List<CustomerFeedback>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
