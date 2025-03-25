using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using Google.Api.Gax.ResourceNames;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace DACS_WebClothes.Controllers
{
    public class CommentProductController : Controller
    {
        private readonly WebClothesContext _context;
        public CommentProductController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> _MenuPartial()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            return PartialView(menus);
        }
        public async Task<IActionResult> AddCP(int ProductId)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();

            var productName = await _context.Products
                                    .Where(p => p.ProductId == ProductId)
                                    .Select(p => p.Name)
                                    .FirstOrDefaultAsync();
            var productLink = await _context.Products
                                    .Where(p => p.ProductId == ProductId)
                                    .Select(p => p.Link)
                                    .FirstOrDefaultAsync();
            var viewModel = new CommentProductViewModel
            {
                Menus = menus,
                ProductId = ProductId,
                ProductName = productName,
                Link = productLink,
                cp = new CommentProduct { DateBegin = DateTime.Now }
            };
            viewModel.cp.DateBegin = DateTime.Now;
            // Lấy tên người dùng đang đăng nhập
            var userName = User.Identity.Name;

            // Tìm UserId từ bảng Users
            var user = await _context.Users.FirstOrDefaultAsync(x => x.FullName == userName);
            if (user != null)
            {
                ViewBag.UserId = user.UserId;
            }
            else
            {
                ViewBag.UserId = 0; // Hoặc giá trị mặc định nào đó
            }
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCP(CommentProductViewModel fbViewModel)
        {
            ModelState.Remove("Prods");
            ModelState.Remove("cp.User");
            ModelState.Remove("cp.Product");
            ModelState.Remove("User");
            ModelState.Remove("Slug");
            ModelState.Remove("ProductName");
            ModelState.Remove("Link");
            ModelState.Remove("ContentProduct");
            if (ModelState.IsValid)
            {
                var compro = new CommentProduct
                {
                    UserId = fbViewModel.cp.UserId,
                    ProductId = fbViewModel.cp.ProductId,
                    ComProName = fbViewModel.cp.ComProName,
                    DateBegin = fbViewModel.cp.DateBegin
                };

                _context.CommentProducts.Add(compro);
                await _context.SaveChangesAsync();
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == fbViewModel.cp.ProductId);
                if (product != null)
                {
                    string productLink = product.Link;
                    int productId = product.ProductId;

                    // Tạo URL theo định dạng "san-pham/{slug}-{id}"
                    string url = Url.RouteUrl("chi-tiet-san-pham", new { slug = productLink, id = productId });

                    // Chuyển hướng về URL đó
                    return Redirect(url);
                }

            }
            return View(fbViewModel);
        }
        public async Task<IActionResult> EditCP(int ProductId, int CommentId)
        {
            var commentProduct = await _context.CommentProducts
                .Include(cp => cp.User)
                .FirstOrDefaultAsync(cp => cp.ComProId == CommentId);

            if (commentProduct == null)
            {
                return NotFound();
            }

            var productName = await _context.Products
                                    .Where(p => p.ProductId == ProductId)
                                    .Select(p => p.Name)
                                    .FirstOrDefaultAsync();
            var productLink = await _context.Products
                                    .Where(p => p.ProductId == ProductId)
                                    .Select(p => p.Link)
                                    .FirstOrDefaultAsync();
            var viewModel = new CommentProductViewModel
            {
                Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync(),
                ProductId = ProductId,
                ComproId = CommentId,
                ProductName = productName,
                Link = productLink,
                ContentProduct = commentProduct.ComProName,
                cp = commentProduct
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCP(CommentProductViewModel viewModel)
        {
            ModelState.Remove("Prods");
            ModelState.Remove("cp.User");
            ModelState.Remove("cp.UserId");
            ModelState.Remove("Slug");
            ModelState.Remove("cp.Product");
            ModelState.Remove("ProductName");
            ModelState.Remove("Link");
            ModelState.Remove("ContentProduct");
            if (ModelState.IsValid)
            {
                var commentProduct = await _context.CommentProducts.FirstOrDefaultAsync(cp => cp.ComProId == viewModel.ComproId);

                if (commentProduct == null)
                {
                    return NotFound();
                }
                commentProduct.ComProName = viewModel.cp.ComProName;
                try
                {
                    _context.Update(commentProduct);
                    await _context.SaveChangesAsync();
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == viewModel.cp.ProductId);
                    if (product != null)
                    {
                        string productLink = product.Link;
                        int productId = product.ProductId;

                        // Tạo URL theo định dạng "san-pham/{slug}-{id}"
                        string url = Url.RouteUrl("chi-tiet-san-pham", new { slug = productLink, id = productId });

                        // Chuyển hướng về URL đó
                        return Redirect(url);
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(viewModel);
        }


        public async Task<IActionResult> DeleteCP(int ProductId, int CommentId)
        {
            var commentProduct = await _context.CommentProducts.FindAsync(CommentId);
            if (commentProduct == null)
            {
                return NotFound();
            }

            var productName = await _context.Products
                                    .Where(p => p.ProductId == ProductId)
                                    .Select(p => p.Name)
                                    .FirstOrDefaultAsync();
            var productLink = await _context.Products
                                    .Where(p => p.ProductId == ProductId)
                                    .Select(p => p.Link)
                                    .FirstOrDefaultAsync();
            var viewModel = new CommentProductViewModel
            {
                Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync(),
                ProductId = ProductId,
                ComproId = CommentId,
                ProductName = productName,
                Link = productLink,
                ContentProduct = commentProduct.ComProName,
                cp = commentProduct
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCP(CommentProductViewModel viewModel)
        {
            ModelState.Remove("Prods");
            ModelState.Remove("cp.User");
            ModelState.Remove("cp.UserId");
            ModelState.Remove("Slug");
            ModelState.Remove("cp.Product");
            ModelState.Remove("ProductName");
            ModelState.Remove("Link");
            ModelState.Remove("ContentProduct");
            if (ModelState.IsValid)
            {
                var commentProduct = await _context.CommentProducts.FindAsync(viewModel.ComproId);

                if (commentProduct == null)
                {
                    return NotFound();
                }

                try
                {
                    _context.CommentProducts.Remove(commentProduct);
                    await _context.SaveChangesAsync();
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == viewModel.cp.ProductId);
                    if (product != null)
                    {
                        string productLink = product.Link;
                        int productId = product.ProductId;

                        // Tạo URL theo định dạng "san-pham/{slug}-{id}"
                        string url = Url.RouteUrl("chi-tiet-san-pham", new { slug = productLink, id = productId });

                        // Chuyển hướng về URL đó
                        return Redirect(url);
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Unable to delete comment. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(viewModel);
        }

    }
}
