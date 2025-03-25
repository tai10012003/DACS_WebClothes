using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACS_WebClothes.Models;
using System.Security.Claims;
using DACS_WebClothes.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DACS_WebClothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class AdminInventoriesController : Controller
    {
        private readonly WebClothesContext _context;

        public AdminInventoriesController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.Inventories.Include(p => p.Product).Where(x => x.PlaceOfImport.ToUpper().Contains(SearchString.ToUpper()));
                return View(searchedData);
            }
            var webclothesContext = await _context.Inventories
                .Include(p => p.Product)
                .ToListAsync();
            return View(webclothesContext);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kh = await _context.Inventories
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (kh == null)
            {
                return NotFound();
            }

            return View(kh);
        }
        public async Task<IActionResult> Create()
        {
            var prod = _context.Products.ToList();
            ViewBag.Products = new SelectList(prod, "ProductId", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventory kh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kh);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminInventories/Index");
            }
            var prod = _context.Products.ToList();
            ViewBag.Products = new SelectList(prod, "ProductId", "Name");
            return View(kh);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kh = await _context.Inventories.FindAsync(id);
            if (kh == null)
            {
                return NotFound();
            }
            var prod = _context.Products.ToList();
            ViewBag.Products = new SelectList(prod, "ProductId", "Name");
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inventory kh)
        {

            if (id != kh.InventoryId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _context.Update(kh);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminInventories/Index");
            }
            var prod = _context.Products.ToList();
            ViewBag.Products = new SelectList(prod, "ProductId", "Name", kh.InventoryId);
            return View(kh);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kh = await _context.Inventories
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (kh == null)
            {
                return NotFound();
            }

            return View(kh);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kh = await _context.Inventories.FindAsync(id);
            if (kh != null)
            {
                _context.Inventories.Remove(kh);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminInventories/Index");
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.InventoryId == id);
        }
    }
}
