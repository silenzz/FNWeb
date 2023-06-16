using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FNWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => {
            options.LoginPath = "/Account/Login"; // specify login page
            options.AccessDeniedPath = "/Account/Forbidden"; // specify forbidden page
        });
builder.Services.AddDbContext<DB>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthorization();
// Add middleware to delete data from table
/*app.Use(async (context, next) =>
{
    // Get the instance of the DbContext
    var dbContext = context.RequestServices.GetService<DB>();
    // Access the table you want to delete data from
    var table = dbContext.Set<YourTable>();
    // Delete all rows from the table
    await table.ForEachAsync(x => dbContext.Entry(x).State = EntityState.Deleted);
    await dbContext.SaveChangesAsync();
    // Call the next middleware
    await next.Invoke();
});*/
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
