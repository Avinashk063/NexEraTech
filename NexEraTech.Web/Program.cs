using Microsoft.EntityFrameworkCore;
using NexEraTech.Application.Interface;
using NexEraTech.Application.Service;
using NexEraTech.Domain.Models.Settings;
using NexEraTech.Infrastructure.Data;
using NexEraTech.Infrastructure.Data.Interface;
using NexEraTech.Infrastructure.Repository;
using NexEraTech.Infrastructure.Repository.Interface;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<NexEraTechDB>(options =>
    options.UseNpgsql(builder.Configuration.GetSection("Settings:DBSettings:ConnectionString").Value));

// Bind EmailSettings from configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Settings:EmailSettings"));

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INexEraTechRepository, NexEraTechRepository>();
builder.Services.AddScoped<INexEraTechDB, NexEraTechDB>();

var app = builder.Build();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS intentionally omitted (handled by Nginx)
}

// Enable static files (CSS, JS, images)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configure default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Environment-based port binding
if (app.Environment.IsDevelopment())
{
    app.Run();
}
else
{
    app.Run("http://0.0.0.0:5000");
}