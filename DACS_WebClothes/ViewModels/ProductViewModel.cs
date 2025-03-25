using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class ProductViewModel : List<ProductViewModel>
    {
        public List<Menu> Menus { get; set; }
        public List<Product> Prods { get; set; }
        public List<CommentProduct> cp { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int CartItemsCount { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public Product CurrentProduct { get; set; } 
        public List<Product> RelatedProducts { get; set; } 
        public string Size { get; set; }
        public String CateName { get; set; }
        public Category Category { get; set; }
        public List<Category> Categories { get; set; }
        public string Image { get; set; }
        public string ProductName { get; set; }
        public int Soldnum { get; set; }
        public decimal Price { get; set; }

    }
}
