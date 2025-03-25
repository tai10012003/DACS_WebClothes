using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class HomeViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public Dictionary<Category, List<Product>> CategoryProducts { get; set; }
        public int ProductId { get; set; }
        public Category Category { get; set; }
    }
}
