var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddHttpContextAccessor();

// Register each Db class for dependency injection
builder.Services.AddScoped<HelpdeskApp.Data.UserDb>();
builder.Services.AddScoped<HelpdeskApp.Data.CategoryDb>();
builder.Services.AddScoped<HelpdeskApp.Data.TicketDb>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tickets}/{action=Index}/{id?}");

app.Run();