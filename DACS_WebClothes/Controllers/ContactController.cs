using Microsoft.AspNetCore.Mvc;
using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DACS_WebClothes.Controllers
{
    public class ContactController : Controller
    {
        private readonly WebClothesContext _context;
        public ContactController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new ContactViewModel
            {
                Menus = menus,
            };
            return View(viewModel);
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
    }
}
