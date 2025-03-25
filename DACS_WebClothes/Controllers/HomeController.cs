using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DACS_WebClothes.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebClothesContext _context;
        public HomeController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(4).ToListAsync();
            var cate = await _context.Categories.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new HomeViewModel
            {
                Menus = menus,
                Blogs = blogs,
                Categories = cate,
                CategoryProducts = new Dictionary<Category, List<Product>>()
            };
            var categories = await _context.Categories.ToListAsync();
            foreach (var category in categories)
            {
                // Lấy tất cả sản phẩm thuộc danh mục hiện tại
                var products = await _context.Products
                    .Where(m => m.Hidden == 0 && m.CategoryId == category.CategoryId).Take(4)
                    .ToListAsync();

                // Thêm danh sách sản phẩm vào viewModel
                viewModel.CategoryProducts.Add(category, products);
            }

            return View(viewModel);
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _CategoriesPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _BlogPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _ProductPartial()
        {
            return PartialView();
        }
    }

}
