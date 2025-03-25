using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;

namespace DACS_WebClothes.Controllers
{
    public class CommentBlogController : Controller
    {
        private readonly WebClothesContext _context;
        public CommentBlogController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> _MenuPartial()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            return PartialView(menus);
        }
        public async Task<IActionResult> AddCB(int BlogId)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var blogName = await _context.Blogs
                                    .Where(p => p.BlogId == BlogId)
                                    .Select(p => p.Title)
                                    .FirstOrDefaultAsync();
            var blogLink = await _context.Blogs
                                    .Where(p => p.BlogId == BlogId)
                                    .Select(p => p.Link)
                                    .FirstOrDefaultAsync();
            var viewModel = new CommentBlogViewModel
            {
                Menus = menus,
                BlogId = BlogId,
                ContentBlog = blogName,
                Link = blogLink,
                cb = new CommentBlog { DateBegin = DateTime.Now }
            };
            viewModel.cb.DateBegin = DateTime.Now;
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
        public async Task<IActionResult> AddCB(CommentBlogViewModel fbViewModel)
        {
            ModelState.Remove("Blogs");
            ModelState.Remove("cb.User");
            ModelState.Remove("cb.UserId");
            ModelState.Remove("Slug");
            ModelState.Remove("cb.Blog");
            ModelState.Remove("Link");
            ModelState.Remove("ContentBlog");
            ModelState.Remove("BlogName");

            if (ModelState.IsValid)
            {
                var comblo = new CommentBlog
                {
                    UserId = fbViewModel.cb.UserId,
                    BlogId = fbViewModel.cb.BlogId,
                    ComBlogName = fbViewModel.cb.ComBlogName,
                    DateBegin = fbViewModel.cb.DateBegin
                };

                _context.CommentBlogs.Add(comblo);
                await _context.SaveChangesAsync();

                var blog = await _context.Blogs.FirstOrDefaultAsync(p => p.BlogId == fbViewModel.cb.BlogId);
                if (blog != null)
                {
                    string blogLink = blog.Link;
                    int blogId = blog.BlogId;
                    string url = Url.RouteUrl("chi-tiet-bai-viet", new { slug = blogLink, id = blogId });

                    // Chuyển hướng về URL đó
                    return Redirect(url);
                }
            }

            return View(fbViewModel);
        }
        public async Task<IActionResult> EditCB(int BlogId, int CommentId)
        {
            var commentBlog = await _context.CommentBlogs
                .Include(cp => cp.User)
                .FirstOrDefaultAsync(cp => cp.ComBlogId == CommentId);
            if (commentBlog == null)
            {
                return NotFound();
            }
            var blogName = await _context.Blogs
                                    .Where(p => p.BlogId == BlogId)
                                    .Select(p => p.Title)
                                    .FirstOrDefaultAsync();
            var blogLink = await _context.Blogs
                                    .Where(p => p.BlogId == BlogId)
                                    .Select(p => p.Link)
                                    .FirstOrDefaultAsync();

            var viewModel = new CommentBlogViewModel
            {
                Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync(),
                BlogId = BlogId,
                ComBlogId = CommentId,
                BlogName = blogName,
                ContentBlog = commentBlog.ComBlogName,
                Link = blogLink,
                cb = commentBlog
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCB(CommentBlogViewModel viewModel)
        {
            ModelState.Remove("Blogs");
            ModelState.Remove("cb.User");
            ModelState.Remove("cb.UserId");
            ModelState.Remove("Slug");
            ModelState.Remove("cb.Blog");
            ModelState.Remove("ContentBlog");
            ModelState.Remove("Link");
            ModelState.Remove("BlogName");
            if (ModelState.IsValid)
            {
                var commentBlog = await _context.CommentBlogs.FirstOrDefaultAsync(cp => cp.ComBlogId == viewModel.ComBlogId);

                if (commentBlog == null)
                {
                    return NotFound();
                }
                commentBlog.ComBlogName = viewModel.cb.ComBlogName;
                try
                {
                    _context.Update(commentBlog );
                    await _context.SaveChangesAsync();
                    var blog = await _context.Blogs.FirstOrDefaultAsync(p => p.BlogId == viewModel.cb.BlogId);
                    if (blog != null)
                    {
                        string blogLink = blog.Link;
                        int blogId = blog.BlogId;
                        string url = Url.RouteUrl("chi-tiet-bai-viet", new { slug = blogLink, id = blogId });

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


        public async Task<IActionResult> DeleteCB(int BlogId, int CommentId)
        {
            var commentBlog = await _context.CommentBlogs.FindAsync(CommentId);
            if (commentBlog == null)
            {
                return NotFound();
            }
            var blogName = await _context.Blogs
                                    .Where(p => p.BlogId == BlogId)
                                    .Select(p => p.Title)
                                    .FirstOrDefaultAsync();
            var blogLink = await _context.Blogs
                                    .Where(p => p.BlogId == BlogId)
                                    .Select(p => p.Link)
                                    .FirstOrDefaultAsync();
            var viewModel = new CommentBlogViewModel
            {
                Menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync(),
                BlogId = BlogId,
                ComBlogId = CommentId,
                BlogName = blogName,
                ContentBlog = commentBlog.ComBlogName,
                Link = blogLink,
                cb = commentBlog
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCB(CommentBlogViewModel viewModel)
        {
            ModelState.Remove("Blogs");
            ModelState.Remove("cb.User");
            ModelState.Remove("cb.UserId");
            ModelState.Remove("Slug");
            ModelState.Remove("cb.Blog");
            ModelState.Remove("Link");
            ModelState.Remove("ContentBlog");
            ModelState.Remove("BlogName");
            if (ModelState.IsValid)
            {
                var commentBlog = await _context.CommentBlogs.FindAsync(viewModel.ComBlogId);

                if (commentBlog == null)
                {
                    return NotFound();
                }

                try
                {
                    _context.CommentBlogs.Remove(commentBlog);
                    await _context.SaveChangesAsync();
                    var blog = await _context.Blogs.FirstOrDefaultAsync(p => p.BlogId == viewModel.cb.BlogId);
                    if (blog != null)
                    {
                        string blogLink = blog.Link;
                        int blogId = blog.BlogId;
                        string url = Url.RouteUrl("chi-tiet-bai-viet", new { slug = blogLink, id = blogId });

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
