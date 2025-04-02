using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TCSA_Movies.Arashi256.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddDbContext<TCSA_MoviesArashi256Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TCSA_MoviesArashi256Context") ?? throw new InvalidOperationException("Connection string 'TCSA_MoviesArashi256Context' not found.")));
// Add Identity services to the container.
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TCSA_MoviesArashi256Context>();
// Add services to the container.
builder.Services.AddControllersWithViews();
var app = builder.Build();
// Seed the database with initial data if it doesn't exist.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TCSA_MoviesArashi256Context>();
        // Ensure DB and tables are created
        context.Database.EnsureCreated();
        // Seed movies if none exist
        if (!context.Movie.Any())
        {
            context.Movie.AddRange(
                new Movie { Title = "The Matrix", Genre = "Sci-Fi", Price = 9.99M, Rating = 8.7M, ReleaseDate = new DateTime(1999, 3, 31) },
                new Movie { Title = "The Godfather", Genre = "Crime", Price = 12.50M, Rating = 9.2M, ReleaseDate = new DateTime(1972, 3, 24) },
                new Movie { Title = "Inception", Genre = "Sci-Fi", Price = 11.00M, Rating = 8.8M, ReleaseDate = new DateTime(2010, 7, 16) },
                new Movie { Title = "Gladiator", Genre = "Action", Price = 10.00M, Rating = 8.5M, ReleaseDate = new DateTime(2000, 5, 5) },
                new Movie { Title = "Pulp Fiction", Genre = "Crime", Price = 8.99M, Rating = 8.9M, ReleaseDate = new DateTime(1994, 10, 14) }
            );
            context.SaveChanges();
            Log.Information("Database seeded with sample movies.");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred seeding the database.");
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    context.Response.Headers["Surrogate-Control"] = "no-store";
    await next();
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");
try
{
    Log.Information("Starting web app");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Web app host terminated unexpectedly");
}
finally
{
    Log.Information("Stopping web app");
    Log.CloseAndFlush();
}