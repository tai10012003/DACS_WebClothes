using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACS_WebClothes.Models;

namespace WebBG.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminPermission")]
    public class AdminHomeCommentController : Controller
    {
        private readonly WebClothesContext _context;

        public AdminHomeCommentController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int numberOfCommentPro = await _context.CommentProducts.CountAsync();
            int numberOfCommentBlog = await _context.CommentBlogs.CountAsync();

            ViewBag.NumberOfCommentProducts = numberOfCommentPro;
            ViewBag.NumberOfCommentBlogs = numberOfCommentBlog;

            return View();
        }
    }
}
