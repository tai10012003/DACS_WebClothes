using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DACS_WebClothes.Controllers
{
    public class BlogController : Controller
    {
        private readonly WebClothesContext _context;
        public BlogController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int blogsPerPage = 2; // Number of blogs per page
            var totalBlogs = await _context.Blogs.Where(m => m.Hidden == 0).CountAsync();
            var totalPages = (int)Math.Ceiling(totalBlogs / (double)blogsPerPage);

            var blogs = await _context.Blogs
                .Where(m => m.Hidden == 0)
                .OrderBy(m => m.OrderIndex)
                .Skip((page - 1) * blogsPerPage)
                .Take(blogsPerPage)
                .ToListAsync();

            var viewModel = new BlogViewModel
            {
                Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync(),
                Blogs = blogs,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> BlogDetail(int id, string slug)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blog = await _context.Blogs
            .Where(m => m.Link == slug && m.BlogId == id)
            .ToListAsync();
            var baiviet = await _context.Blogs
            .FirstOrDefaultAsync(b => b.Link == slug && b.BlogId == id);

            var listcb = await _context.CommentBlogs
            .Where(cb => cb.BlogId == id)
            .OrderBy(m => m.BlogId)
            .Select(cb => new CommentBlog
            {
                ComBlogId = cb.ComBlogId,
                UserId = cb.UserId,
                BlogId = cb.BlogId,
                ComBlogName = cb.ComBlogName,
                DateBegin = cb.DateBegin,
                User = cb.User,
                Blog = cb.Blog
            })
            .ToListAsync();
            if (blog == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = "Blog Error",
                };
                return View("Error", errorViewModel);
            }
            if (baiviet == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = "Blog Error",
                };
                return View("Error", errorViewModel);
            }
            var relatedBlogs = await _context.Blogs
            .Where(b => b.BlogId != id && b.Content.Contains(baiviet.Content))
            .Take(4) // Lấy tối đa 5 bài viết liên quan
            .ToListAsync();

            var viewModel = new BlogViewModel
            {
                Menus = menus,
                cb = listcb,
                BlogId = id,
                Blogs = new List<Blog> { baiviet }, // Lưu bài viết vào một danh sách mới
                RelatedBlogs = relatedBlogs
            };
            return View(viewModel);
        }
    }
}
