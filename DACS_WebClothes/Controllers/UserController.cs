using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DACS_WebClothes.Models;
using DACS_WebClothes.ViewModels;
using Microsoft.AspNetCore.Authentication.Google;

namespace DACS_WebClothes.Controllers
{
    public class UserController : Controller
    {
        private readonly WebClothesContext _context;
        public UserController(WebClothesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new UserViewModel
            {
                Menus = menus,
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            int userId = model.Register.UserId;
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = model.Register,
            };
            if (model.Register != null)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Register.Username);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại.";
                    return View(viewModel);
                }
                model.Register.Password = BCrypt.Net.BCrypt.HashPassword(model.Register.Password);
                model.Register.Permission = 0;
                model.Register.Hidden = 0;
                _context.Users.Add(model.Register);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "User");
            }
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new UserViewModel
            {
                Menus = menus,
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = model.Register,
            };
            if (model.Register != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Register.Username);
                if (user.Hidden == 1)
                {
                    ViewBag.ErrorMessage = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên để biết thêm thông tin.";
                    return View(viewModel);
                }
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Register.Password, user.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Role, user.Permission.ToString()),
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    ViewBag.LoginSuccess = true;
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
            ViewBag.LoginSuccess = false;

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result?.Principal == null)
            {
                return RedirectToAction("Login", "User");
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims.ToList();

            if (claims == null)
            {
                return RedirectToAction("Login", "User");
            }

            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Permission.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Info()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var users = new User();
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    users = await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
                }
            }
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = users,
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var user = await GetUserFromContext();
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = user,
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserViewModel model)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var user = await GetUserFromContext();
            if (user == null)
            {
                return RedirectToAction("Logout", "User");
            }

            user.FullName = model.Register.FullName;
            user.Address = model.Register.Address;
            user.Email = model.Register.Email;
            user.Phone = model.Register.Phone;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Info", "User");
        }

        private async Task<User> GetUserFromContext()
        {
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    return await _context.Users.FirstOrDefaultAsync(m => m.FullName == username);
                }
            }
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var user = await GetUserFromContext();
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = user,
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserViewModel model)
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var user = await GetUserFromContext();
            if (user == null)
            {
                return RedirectToAction("Logout", "User");
            }

            if (model.Register != null)
            {
                if (BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Info", "User");
                }
            }

            ViewBag.ErrorMessage = "Mật khẩu cũ không đúng.";
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = user,
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Settings()
        {
            var menus = await _context.Menus.Where(m => m.Hidden == 0).OrderBy(m => m.OrderIndex).ToListAsync();
            var viewModel = new UserViewModel
            {
                Menus = menus,
            };
            return View(viewModel);
        }

    }

}
