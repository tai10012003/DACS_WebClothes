using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace DACS_WebClothes.Controllers
{
    public class SearchController : Controller
    {
        private readonly WebClothesContext _context;
        public SearchController(WebClothesContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> SearchPro(string searchTerm = "", int page = 1)
        {
            var viewModel = new SearchViewModel();
            viewModel.Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();

            // Tìm kiếm các gợi ý
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var suggestions = await _context.Products
                    .Where(p => p.Name.Contains(searchTerm) || p.Category.CategoryName.Contains(searchTerm) || p.Brand.BrandName.Contains(searchTerm))
                    .Select(p => p.Name)
                    .Take(10)
                    .ToListAsync();

                ViewBag.Suggestions = suggestions;
            }
            else
            {
                ViewBag.Suggestions = new List<string>();
            }

            // Tìm kiếm sản phẩm
            var query = _context.Products.Include(p => p.Category).Include(p => p.Brand).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                         p.Category.CategoryName.Contains(searchTerm) ||
                                         p.Brand.BrandName.Contains(searchTerm) ||
                                         p.Price.ToString().Contains(searchTerm));
            }
            // Pagination logic
            int productsPerPage = 12;
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)productsPerPage);

            var products = await query.Skip((page - 1) * productsPerPage).Take(productsPerPage).ToListAsync();

            viewModel.Prods = products;
            viewModel.SearchTerm = searchTerm;
            viewModel.TotalPages = totalPages;
            viewModel.CurrentPage = page;

            ViewBag.SearchTerm = searchTerm;

            return View("SearchPro", viewModel);
        }

    }
}
