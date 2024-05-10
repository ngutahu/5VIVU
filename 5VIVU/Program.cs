using _5VIVU.Data;
using _5VIVU.Hepler;
using _5VIVU.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using WebAIrline.Services;
using Microsoft.Graph.Models.Security;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSession();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<_5vivuBanveContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("5vivu"));
});

builder.Services.AddSingleton<IVnPayService, VnPayService>();
var MaillSettings = configuration.GetSection("MaillSettings");
builder.Services.Configure<MailSetting>(MaillSettings);
builder.Services.AddSingleton<IEmailSender, SendMailServices>();
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddOptions();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
