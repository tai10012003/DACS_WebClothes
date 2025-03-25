using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class BlogViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<CommentBlog> cb { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int BlogId { get; set; }
        public List<Blog> RelatedBlogs { get; set; }
    }
}
