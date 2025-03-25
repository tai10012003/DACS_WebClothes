using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class CommentBlogViewModel
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();
        public List<Blog> Blogs { get; set; }
        public int BlogId { get; set; }
        public string ContentBlog { get; set; }
        public string BlogName { get; set; }
        public int ComBlogId { get; set; }
        public CommentBlog cb { get; set; }
        public string Link { get; set; }
        public string Slug { get; set; }
        public CommentBlogViewModel()
        {
            cb = new CommentBlog();
        }
    }
}
