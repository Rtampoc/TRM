using Microsoft.EntityFrameworkCore;
using TRM.Data;
using TRM.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add Entity Framework Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register authentication service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Register user management service
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// Use session middleware
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Set login page as the default startup page
app.MapGet("/", async context =>
{
    var userSession = context.Session.GetString("UserId");
    if (string.IsNullOrEmpty(userSession))
    {
        context.Response.Redirect("/Auth/Login");
    }
    else
    {
        context.Response.Redirect("/Index");
    }
    await Task.CompletedTask;
});

app.Run();
