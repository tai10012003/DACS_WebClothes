using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class UserViewModel
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();
        public User Register { get; set; }
        public string OldPassword { get; set; } // Add this property
        public string NewPassword { get; set; } // Add this property

        public UserViewModel()
        {
            Register = new User();
        }
    }
}
