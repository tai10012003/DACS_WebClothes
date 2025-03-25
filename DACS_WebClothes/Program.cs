using DACS_WebClothes.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DACS_WebClothes.Authorization;
using DACS_WebClothes.Services;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "ClothesStoreCookie";
    options.LoginPath = "/User/Login";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});
builder.Services.AddSession();
var connectionString = builder.Configuration.GetConnectionString("WebsiteBanHangConnection");
builder.Services.AddDbContext<WebClothesContext>(options => options.UseSqlServer(connectionString));


builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddCustomPolicies();

builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorOptions(options =>
{
    options.ViewLocationFormats.Add("/Areas/Admin/Views/Home/Index.cshtml");
});
builder.Services.AddSingleton<IVnPayService, VnPayService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
     name: "trang-chu",
     pattern: "trang-chu",
     defaults: new { controller = "Home", action = "Index" });

    endpoints.MapControllerRoute(
     name: "lien-he",
     pattern: "lien-he",
     defaults: new { controller = "Contact", action = "Index" });

    endpoints.MapControllerRoute(
     name: "cart",
     pattern: "cart",
     defaults: new { controller = "Cart", action = "Index" });

    endpoints.MapControllerRoute(
     name: "AddCart",
     pattern: "AddCart",
     defaults: new { controller = "Cart", action = "AddItem" });

    endpoints.MapControllerRoute(
     name: "order",
     pattern: "order",
     defaults: new { controller = "Cart", action = "Order" });

    endpoints.MapControllerRoute(
    name: "hoan-thanh",
    pattern: "hoan-thanh",
    defaults: new { controller = "Cart", action = "Success" });

    endpoints.MapControllerRoute(
    name: "don-hang-cua-toi",
    pattern: "don-hang-cua-toi",
    defaults: new { controller = "Cart", action = "UserOrders" });

    endpoints.MapControllerRoute(
     name: "dang-ky",
     pattern: "dang-ky",
     defaults: new { controller = "User", action = "Register" });

    endpoints.MapControllerRoute(
     name: "dang-nhap",
     pattern: "dang-nhap",
     defaults: new { controller = "User", action = "Login" });

    endpoints.MapControllerRoute(
     name: "thong-tin",
     pattern: "thong-tin",
     defaults: new { controller = "User", action = "Info" });

    endpoints.MapControllerRoute(
     name: "setting",
     pattern: "setting",
     defaults: new { controller = "User", action = "Settings" });

    endpoints.MapControllerRoute(
     name: "dang-xuat",
     pattern: "dang-xuat",
     defaults: new { controller = "User", action = "Logout" });

    endpoints.MapControllerRoute(
     name: "san-pham",
     pattern: "san-pham",
     defaults: new { controller = "Product", action = "Index" });

    endpoints.MapControllerRoute(
     name: "chi-tiet-san-pham",
     pattern: "san-pham/{slug}-{id}",
     defaults: new { controller = "Product", action = "ProdDetail" });

    endpoints.MapControllerRoute(
     name: "sales",
     pattern: "sales",
     defaults: new { controller = "Product", action = "SalesProduct" });

    endpoints.MapControllerRoute(
    name: "AddCP",
    pattern: "AddCP",
    defaults: new { controller = "CommentProduct", action = "AddCP" });

    endpoints.MapControllerRoute(
    name: "bai-viet",
    pattern: "bai-viet",
    defaults: new { controller = "Blog", action = "Index" });

    endpoints.MapControllerRoute(
    name: "chi-tiet-bai-viet",
    pattern: "bai-viet/{slug}-{id}",
    defaults: new { controller = "Blog", action = "BlogDetail" });

    endpoints.MapControllerRoute(
     name: "AddCB",
     pattern: "AddCB",
     defaults: new { controller = "CommentBlog", action = "AddCB" });


});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
   name: "Admin",
   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();
