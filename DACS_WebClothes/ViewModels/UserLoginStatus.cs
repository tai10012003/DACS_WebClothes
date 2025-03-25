using DACS_WebClothes.Models;

namespace DACS_WebClothes.ViewModels
{
    public class UserLoginStatus
    {
        public User User { get; set; }
        public TimeSpan TimeSinceLastLogin { get; set; }
    }
}
