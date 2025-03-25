using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using DACS_WebClothes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DACS_WebClothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class AdminProductController : Controller
    {
        private readonly WebClothesContext _context;

        public AdminProductController(WebClothesContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminBoardGames
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var sanPham = await _context.Products
                    .Include(b => b.Category)
                    .Include(b => b.Brand)
                    .Where(x => x.Name.ToUpper().Contains(SearchString.ToUpper()))
                    .ToListAsync(); // Chuyển sang ListAsync() để lấy danh sách
                return View(sanPham);
            }
            var webclothesContext = await _context.Products
                .Include(b => b.Category)
                .Include(b => b.Brand)
                .ToListAsync(); // Chuyển sang ListAsync() để lấy danh sách
            return View(webclothesContext);
        }


        // GET: Admin/AdminBoardGames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prod = await _context.Products
                .Include(b => b.Category)
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (prod == null)
            {
                return NotFound();
            }

            return View(prod);
        }
        public IActionResult Create()
        {
            var categories =  _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var brands = _context.Brands.ToList();
            ViewBag.Brands = new SelectList(brands, "BrandId", "BrandName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile image1, IFormFile image2, IFormFile image3)
        {
            if (ModelState.IsValid)
            {
                if (image1 != null)
                {
                    product.Image1 = await SaveImage(image1);
                }
                if (image2 != null)
                {
                    product.Image2 = await SaveImage(image2);
                }
                if (image3 != null)
                {
                    product.Image3 = await SaveImage(image3);
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminProduct/Index");
            }
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var brands = _context.Brands.ToList();
            ViewBag.Brands = new SelectList(brands, "BrandId", "BrandName");
            return View(product);
        }

        // GET: Admin/AdminBoardGames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prods = await _context.Products.FindAsync(id);
            if (prods == null)
            {
                return NotFound();
            }
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var brands = _context.Brands.ToList();
            ViewBag.Brands = new SelectList(brands, "BrandId", "BrandName");
            return View(prods);
        }

        // POST: Admin/AdminBoardGames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile image1, IFormFile image2, IFormFile image3)
        {
            ModelState.Remove("image1");
            ModelState.Remove("image2");
            ModelState.Remove("image3");
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingpro = await _context.Products.FindAsync(id); // 
                    //neu hinh anh ban null thi gan no bang hinh anh cu
                    if (image1 == null)
                    {
                        product.Image1 = existingpro.Image1;
                    }
                    else
                    {
                        // Lưu hình ảnh mới
                        product.Image1 = await SaveImage(image1);
                    }
                    //neu hinh anh ban null thi gan no bang hinh anh cu
                    if (image2 == null)
                    {
                        product.Image2 = existingpro.Image2;
                    }
                    else
                    {
                        // Lưu hình ảnh mới
                        product.Image2 = await SaveImage(image2);
                    }
                    //neu hinh anh ban null thi gan no bang hinh anh cu
                    if (image3 == null)
                    {
                        product.Image3 = existingpro.Image3;
                    }
                    else
                    {
                        // Lưu hình ảnh mới
                        product.Image3 = await SaveImage(image3);
                    }
                    existingpro.Name = product.Name;
                    existingpro.Description = product.Description;
                    existingpro.Image1 = product.Image1;
                    existingpro.Image2 = product.Image2;
                    existingpro.Image3 = product.Image3;
                    existingpro.Price = product.Price;
                    existingpro.OrderIndex = product.OrderIndex;
                    existingpro.Link = product.Link;
                    existingpro.CategoryId = product.CategoryId;
                    existingpro.BrandId = product.BrandId;
                    
                    _context.Products.Update(existingpro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardGameExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Admin/AdminProduct/Index");
            }
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName",product.CategoryId);
            var brands = _context.Brands.ToList();
            ViewBag.Brands = new SelectList(brands, "BrandId", "BrandName", product.BrandId);
            return View(product);
        }

        // GET: Admin/AdminBoardGames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Category)
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminBoardGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminProduct/Index");
        }

        private bool BoardGameExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        //Ham luu hinh anh
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
    }
}
