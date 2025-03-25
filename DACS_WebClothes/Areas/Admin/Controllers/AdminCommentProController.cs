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
    public class AdminCommentProController : Controller
    {
        private readonly WebClothesContext _context;

        public AdminCommentProController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.CommentProducts.Include(p => p.Product).Where(x => x.ComProName.ToUpper().Contains(SearchString.ToUpper()));
                return View(searchedData);
            }
            var webclothesContext = await _context.CommentProducts
                .Include(p => p.Product)
                .Include(p => p.User)
                .ToListAsync();
            return View(webclothesContext);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compro = await _context.CommentProducts
                .Include(p => p.Product)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ComProId == id);
            if (compro == null)
            {
                return NotFound();
            }

            return View(compro);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compro = await _context.CommentProducts
                .FirstOrDefaultAsync(m => m.ComProId == id);
            if (compro == null)
            {
                return NotFound();
            }

            return View(compro);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compro = await _context.CommentProducts.FindAsync(id);
            if (compro != null)
            {
                _context.CommentProducts.Remove(compro);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminCommentPro/Index");
        }
    }
}
