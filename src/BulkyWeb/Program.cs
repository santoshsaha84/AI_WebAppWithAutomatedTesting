using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBookWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Debug logging for Render (SAFE: only shows prefix and length)
if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine($"Connection string detected. Length: {connectionString.Length}. Starts with: {(connectionString.Length > 10 ? connectionString.Substring(0, 10) : connectionString)}");
}
else
{
    Console.WriteLine("No connection string found in DefaultConnection configuration.");
}

// Fix for Render.com: Convert postgres/postgresql URI to Npgsql Connection String
if (!string.IsNullOrEmpty(connectionString) && 
    (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) || 
     connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)))
{
    Console.WriteLine("PostgreSQL URI detected. Converting to Npgsql format...");
    try 
    {
        connectionString = connectionString.Trim();
        var databaseUri = new Uri(connectionString);
        var userInfo = databaseUri.UserInfo.Split(':');
        
        var npgsqlBuilder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
            Username = userInfo[0],
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = Npgsql.SslMode.Require,
            TrustServerCertificate = true
        };
        connectionString = npgsqlBuilder.ToString();
        Console.WriteLine("Conversion successful.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error parsing PostgreSQL URI: {ex.GetType().Name} - {ex.Message}");
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


SeedDatabase();

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (db.Database.GetPendingMigrations().Count() > 0)
        {
            db.Database.Migrate();
        }
    }
}
