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
    public class AdminCommentBlogController : Controller
    {
        private readonly WebClothesContext _context;

        public AdminCommentBlogController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                var searchedData = _context.CommentBlogs.Include(p => p.Blog).Where(x => x.ComBlogName.ToUpper().Contains(SearchString.ToUpper()));
                return View(searchedData);
            }
            var webclothesContext = await _context.CommentBlogs
                .Include(p => p.Blog)
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

            var comblog = await _context.CommentBlogs
                .Include(p => p.Blog)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ComBlogId == id);
            if (comblog == null)
            {
                return NotFound();
            }

            return View(comblog);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comblog = await _context.CommentBlogs
                .FirstOrDefaultAsync(m => m.ComBlogId == id);
            if (comblog == null)
            {
                return NotFound();
            }

            return View(comblog);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comblog = await _context.CommentBlogs.FindAsync(id);
            if (comblog != null)
            {
                _context.CommentBlogs.Remove(comblog);
            }

            await _context.SaveChangesAsync();
            return Redirect("~/Admin/AdminCommentPro/Index");
        }
    }
}
