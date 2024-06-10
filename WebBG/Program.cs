using WebBG.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using WebBG.Authorization;
using WebBG.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
AddCookie(options =>
{
    options.Cookie.Name = "BoardGameStoreCookie";
    options.LoginPath = "/User/Login";
});
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var connectionString = builder.Configuration.GetConnectionString("WebsiteBanHangConnection");
builder.Services.AddDbContext<WebBgContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddCustomPolicies();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorOptions(options =>
{
    options.ViewLocationFormats.Add("/Areas/Admin/Views/Home/Index.cshtml");
});

//dang ki vnpay
builder.Services.AddSingleton<IVnPayService, VnPayService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
     name: "trang-chu",
     pattern: "trang-chu",
     defaults: new { controller = "Home", action = "Index" });

    

    endpoints.MapControllerRoute(
    name: "lien-he",
    pattern: "lien-he",
    defaults: new { controller = "Contact", action = "Index" });

    endpoints.MapControllerRoute(
   name: "payment-fail",
   pattern: "payment-fail",
   defaults: new { controller = "Cart", action = "PaymentFail" });

    endpoints.MapControllerRoute(
    name: "san-pham",
    pattern: "san-pham",
    defaults: new { controller = "Product", action = "Index" });

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
     name: "dang-xuat",
     pattern: "dang-xuat",
     defaults: new { controller = "User", action = "Logout" });

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
    name: "chi-tiet-san-pham",
    pattern: "san-pham/{slug}-{id}",
    defaults: new { controller = "Product", action = "ProdDetail" });

    endpoints.MapControllerRoute(
       name: "xac-nhan-huy",
       pattern: "xac-nhan-huy/{id}",
       defaults: new { controller = "Cart", action = "CancelOrderConfirm" });

    endpoints.MapControllerRoute(
      name: "huy-don-hang",
      pattern: "huy-don-hang/{id}",
      defaults: new { controller = "Cart", action = "CancelOrder" });

    endpoints.MapControllerRoute(
     name: "AddFB",
     pattern: "AddFB",
     defaults: new { controller = "FeedBack", action = "AddFB" });

    endpoints.MapControllerRoute(
    name: "the-loai-san-pham",
    pattern: "{slug}-{id}",
    defaults: new { controller = "Product", action = "CateProd" });

    endpoints.MapControllerRoute(
    name: "blog",
    pattern: "blog",
    defaults: new { controller = "Blog", action = "Index" });

    endpoints.MapControllerRoute(
    name: "chi-tiet-bai-viet",
    pattern: "blog/{slug}-{id}",
    defaults: new { controller = "Blog", action = "BlogDetail" });

    endpoints.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


});
app.MapControllerRoute(
   name: "Admin",
   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");



app.Run();
