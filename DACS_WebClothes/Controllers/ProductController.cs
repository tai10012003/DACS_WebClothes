using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DACS_WebClothes.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebClothesContext _context;
        public ProductController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1, string sort = "")
        {
            ViewBag.Sort = sort;
            int productsPerPage = 12;
            var totalCount = await _context.Products.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)productsPerPage);

            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            List<Product> products;

            switch (sort)
            {
                case "nameAsc":
                    products = await _context.Products.OrderBy(p => p.Name).Skip((page - 1) * productsPerPage).Take(productsPerPage).ToListAsync();
                    break;
                case "nameDesc":
                    products = await _context.Products.OrderByDescending(p => p.Name).Skip((page - 1) * productsPerPage).Take(productsPerPage).ToListAsync();
                    break;
                case "priceAsc":
                    products = await _context.Products.OrderBy(p => p.Price).Skip((page - 1) * productsPerPage).Take(productsPerPage).ToListAsync();
                    break;
                case "priceDesc":
                    products = await _context.Products.OrderByDescending(p => p.Price).Skip((page - 1) * productsPerPage).Take(productsPerPage).ToListAsync();
                    break;
                default:
                    products = await _context.Products.OrderBy(p => p.ProductId).Skip((page - 1) * productsPerPage).Take(productsPerPage).ToListAsync();
                    break;
            }

            var viewModel = new ProductViewModel
            {
                Menus = menus,
                Prods = products,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(viewModel);
        }
        public async Task<IActionResult> CateProd(int categoryId, int page = 1, string sort = "")
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogs = await _context.Blogs.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).Take(2).ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            var category = await _context.Categories.FindAsync(categoryId);
            ViewBag.Sort = sort;
            ViewBag.CategoryId = categoryId;
            int productsPerPage = 12;
            var totalCount = await _context.Products.Where(p => p.CategoryId == categoryId && p.Hidden == 0).CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)productsPerPage);

            if (category == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = "CateProdByCategoryId Error",
                };
                return View("Error", errorViewModel);
            }

            IQueryable<Product> productQuery = _context.Products
                .Where(m => m.Hidden == 0 && m.CategoryId == categoryId);

            switch (sort)
            {
                case "nameAsc":
                    productQuery = productQuery.OrderBy(p => p.Name);
                    break;
                case "nameDesc":
                    productQuery = productQuery.OrderByDescending(p => p.Name);
                    break;
                case "priceAsc":
                    productQuery = productQuery.OrderBy(p => p.Price);
                    break;
                case "priceDesc":
                    productQuery = productQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    productQuery = productQuery.OrderBy(p => p.ProductId);
                    break;
            }

            var products = await productQuery.Skip((page - 1) * productsPerPage).Take(productsPerPage).ToListAsync();

            var viewModel = new ProductViewModel
            {
                Menus = menus,
                Prods = products,
                CateName = category.CategoryName,
                Categories = categories,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(viewModel);
        }
        public async Task<IActionResult> ProdDetail(string slug, int id)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var prods = await _context.Products
                .Include(bg => bg.Brand)
                .Include(bg => bg.Category)
                .Where(m => m.Link == slug && m.ProductId == id)
                .ToListAsync();
            var listcp = await _context.CommentProducts
            .Where(cp => cp.ProductId == id)
            .OrderBy(m => m.ProductId)
            .Select(cp => new CommentProduct
            {
                ComProId = cp.ComProId,
                UserId = cp.UserId,
                ProductId = cp.ProductId,
                ComProName = cp.ComProName,
                DateBegin = cp.DateBegin,
                User = cp.User,
                Product = cp.Product
            })
            .ToListAsync();
            if (!prods.Any())
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = "Product Error",
                };
                return View("Error", errorViewModel);
            }
            var currentProduct = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Link == slug && p.ProductId == id);

            if (currentProduct == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = "Product Error",
                };
                return View("Error", errorViewModel);
            }
            var relatedProducts = await _context.Products
            .Where(p => p.CategoryId == currentProduct.CategoryId && p.ProductId != id).Take(4)
            .ToListAsync();
            var cart = HttpContext.Session.GetString("CartSession");
            var list = new List<CartItem>();
            if (!string.IsNullOrEmpty(cart))
            {
                list = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            }
            var viewModel = new ProductViewModel
            {
                Menus = menus,
                Prods = prods,
                CurrentProduct = currentProduct,
                RelatedProducts = relatedProducts,
                cp = listcp,
                ProductId = id,
                CartItemsCount = list.Count
            };

            return View(viewModel);
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        public async Task<IActionResult> SalesProduct(int page = 1, string sort = "")
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            ViewBag.Sort = sort; // Truyền giá trị sort vào ViewBag

            IQueryable<Product> productQuery = _context.Products.Where(p => p.DiscountPrice != null); // Lấy sản phẩm khuyến mãi
            switch (sort)
            {
                case "nameAsc":
                    productQuery = productQuery.OrderBy(p => p.Name);
                    break;
                case "nameDesc":
                    productQuery = productQuery.OrderByDescending(p => p.Name);
                    break;
                case "priceAsc":
                    productQuery = productQuery.OrderBy(p => p.Price);
                    break;
                case "priceDesc":
                    productQuery = productQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    productQuery = productQuery.OrderBy(p => p.ProductId);
                    break;
            }

            int productsPerPage = 12;
            var totalCount = await productQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)productsPerPage);

            var products = await productQuery.Skip((page - 1) * productsPerPage).Take(productsPerPage).ToListAsync();

            var viewModel = new ProductViewModel
            {
                Menus = menus,
                Prods = products,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(viewModel);
        }
    }
}
