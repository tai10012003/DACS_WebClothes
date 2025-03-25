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
    public class AdminDiscountController : Controller
    {
        private readonly WebClothesContext _context;

        public AdminDiscountController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.Discounts.Where(x => x.DiscountName.ToUpper().Contains(SearchString.ToUpper()));
                return View(searchedData);
            }
            return View(await _context.Discounts.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dc = await _context.Discounts
                .FirstOrDefaultAsync(m => m.DiscountId == id);
            if (dc == null)
            {
                return NotFound();
            }

            return View(dc);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount dc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dc);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminDiscount/Index");
            }
            return View(dc);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dc = await _context.Discounts.FindAsync(id);
            if (dc == null)
            {
                return NotFound();
            }
            return View(dc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Discount dc)
        {

            if (id != dc.DiscountId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _context.Update(dc);
                await _context.SaveChangesAsync();
                return Redirect("~/Admin/AdminDiscount/Index");
            }
            return View(dc);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dc = await _context.Discounts
                .FirstOrDefaultAsync(m => m.DiscountId == id);
            if (dc == null)
            {
                return NotFound();
            }

            return View(dc);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dc = await _context.Discounts.FindAsync(id);
            if (dc != null)
            {
                _context.Discounts.Remove(dc);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminDiscount/Index");
        }

        private bool DcExists(int id)
        {
            return _context.Discounts.Any(e => e.DiscountId == id);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
