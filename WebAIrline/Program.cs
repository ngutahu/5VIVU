using Microsoft.AspNetCore.Identity.UI.Services;
using System.Configuration;
using WebAIrline.Models;
using WebAIrline.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
        builder.WithOrigins("https://localhost:7293", "https://localhost:7129")
               .AllowAnyMethod()
               .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<VietJetContext>();
builder.Services.AddOptions();
var MaillSettings = configuration.GetSection("MaillSettings");
builder.Services.Configure<MailSetting>(MaillSettings);
builder.Services.AddSingleton<IEmailSender, SendMailServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();
app.MapControllers();
//app.UseRouting();
/*app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.
});*/

app.Run();
