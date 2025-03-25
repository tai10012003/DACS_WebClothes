using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class CartViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<CartItem> CartItems { get; set; }
        public User Register { get; set; }
        public Discount dc { get; set; }
        public List<Cart> UserOrders { get; set; }
        public Cart Order { get; set; }

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransportMethod { get; set; }
        public string FullName { get; set; }
        public DateTime DateBegin { get; set; }
        public int NumberOfProducts { get; set; }
        public string Status { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public IEnumerable<CartViewModel> Orders { get; set; }
        public CartViewModel()
        {
            dc = new Discount();
            Order = new Cart();
            Register = new User();
        }
    }
}
