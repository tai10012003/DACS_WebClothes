using Microsoft.AspNetCore.Mvc;

namespace DACS_WebClothes.Areas.Admin
{
    public class AdminStatisticalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
