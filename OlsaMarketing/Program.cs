using Microsoft.EntityFrameworkCore;
using OlsaMarketing.Models;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MarketingDepartment>(options =>
    options.UseSqlServer(connection));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Task}/{id?}");

app.Run();
