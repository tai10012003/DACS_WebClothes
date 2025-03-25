using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class SearchViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<Product> Prods { get; set; }
        public string SearchTerm { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
