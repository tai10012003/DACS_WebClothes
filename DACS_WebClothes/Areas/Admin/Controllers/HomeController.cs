using DACS_WebClothes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DACS_WebClothes.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DACS_WebClothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class HomeController : Controller
    {
        private readonly WebClothesContext _context;

        public HomeController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int numberOfOrders = _context.Carts.Count();
            int numberOfProducts = _context.Products.Count();
            ViewBag.NumberOfOrders = numberOfOrders;
            ViewBag.NumberOfProducts = numberOfProducts;
            var cartDetails = _context.CartDetails.ToList();

            decimal totalRevenue = 0;
            foreach (var cartDetail in cartDetails)
            {
                var cart = await _context.Carts.FindAsync(cartDetail.CartId);
                if (cart != null && cart.Status == "Đã Giao Hàng")
                {
                    var product = await _context.Products.FindAsync(cartDetail.ProductId);
                    if (product != null)
                    {
                        totalRevenue += product.Price.GetValueOrDefault(0) * cartDetail.SoldNum;
                    }
                }
            }
            ViewBag.TotalRevenue = totalRevenue;

            IQueryable<Cart> query = _context.Carts
                .Include(cart => cart.User)
                .Include(cart => cart.CartDetails)
                .Where(cart => cart.Status == "Chờ Xác Nhận");

            var orders = await query
                .OrderByDescending(cart => cart.DateBegin)
                .ToListAsync();

            return View(orders);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var order = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CartId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.User = order.User;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Carts.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            _context.Carts.Update(order);
            await _context.SaveChangesAsync();

            return Redirect("~/Admin/Home/Index");
        }


    }

}
