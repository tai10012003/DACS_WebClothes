using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System;
using DACS_WebClothes.Models;

namespace DACS_WebClothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class AdminStatisticalController : Controller
    {
        private readonly WebClothesContext _context;

        public AdminStatisticalController(WebClothesContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenueStatistics(string period = "month")
        {
            var cartDetails = await _context.CartDetails
                .Include(cd => cd.Cart)
                .Include(cd => cd.Product)
                .ToListAsync();

            var revenueData = cartDetails
                .Where(cd => cd.Cart.Status == "Đã Giao Hàng")
                .GroupBy(cd =>
                {
                    switch (period.ToLower())
                    {
                        case "week":
                            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                            var startOfWeek = cd.Cart.DateBegin.Date;
                            while (startOfWeek.DayOfWeek != firstDayOfWeek)
                                startOfWeek = startOfWeek.AddDays(-1);
                            var endOfWeek = startOfWeek.AddDays(6);
                            return $"{startOfWeek:dd/MM/yyyy} - {endOfWeek:dd/MM/yyyy}";
                        case "year":
                            return cd.Cart.DateBegin.Year.ToString();
                        default:
                            return cd.Cart.DateBegin.Month + "/" + cd.Cart.DateBegin.Year;
                    }
                })
                .Select(g => new
                {
                    Period = g.Key,
                    Revenue = g.Sum(cd => cd.Product.Price.GetValueOrDefault(0) * cd.SoldNum)
                })
                .ToList();

            return Json(revenueData);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
