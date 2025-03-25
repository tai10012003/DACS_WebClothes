using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class CommentProductViewModel
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();
        public List<Product> Prods { get; set; }
        public string ProductName { get; set; }
        public string ContentProduct { get; set; }
        public int ProductId { get; set; }

        public string Link { get; set; }
        public int ComproId { get; set; }
        public CommentProduct cp { get; set; }
        public string Slug { get; set; }   
        public CommentProductViewModel()
        {
            cp = new CommentProduct();
        }
    }
}
