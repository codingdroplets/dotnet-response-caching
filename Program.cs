// Response Caching in ASP.NET Core - Coding Droplets Sample
// This sample demonstrates how to use Response Caching Middleware in ASP.NET Core Minimal APIs

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURE RESPONSE CACHING
// ============================================

// Add Response Caching services
builder.Services.AddResponseCaching();

// Configure cache profiles for reusable caching configuration
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("DefaultCache", new CacheProfile
    {
        Duration = 60,
        Location = ResponseCacheLocation.Any,
        VaryByHeader = "Accept"
    });

    options.CacheProfiles.Add("ShortCache", new CacheProfile
    {
        Duration = 30,
        Location = ResponseCacheLocation.Client
    });

    options.CacheProfiles.Add("LongCache", new CacheProfile
    {
        Duration = 300,
        Location = ResponseCacheLocation.Any,
        VaryByQueryKeys = new[] { "category", "page" }
    });
});

// Add Swagger/OpenAPI for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================
// CONFIGURE MIDDLEWARE PIPELINE
// ============================================

// Enable Response Caching Middleware
app.UseResponseCaching();

// Enable Swagger for API testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Response Caching API v1");
    c.RoutePrefix = "swagger";
});

// ============================================
// SAMPLE ENDPOINTS - Response Caching Demo
// ============================================

// 1. Basic Response Caching - Cache for 60 seconds
app.MapGet("/api/products", () =>
{
    var products = new[]
    {
        new { Id = 1, Name = "Laptop", Price = 999.99, Category = "Electronics" },
        new { Id = 2, Name = "Smartphone", Price = 699.99, Category = "Electronics" },
        new { Id = 3, Name = "Headphones", Price = 149.99, Category = "Audio" },
        new { Id = 4, Name = "Keyboard", Price = 79.99, Category = "Electronics" },
        new { Id = 5, Name = "Mouse", Price = 49.99, Category = "Electronics" }
    };
    return Results.Ok(products);
})
.WithName("GetProducts")
.WithTags("Products")
.WithMetadata(new ResponseCacheAttribute { Duration = 60, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept" });

// 2. Cache with Query String Variation - Different cache for each category value
app.MapGet("/api/products/by-category", (string? category) =>
{
    var allProducts = new[]
    {
        new { Id = 1, Name = "Laptop", Price = 999.99, Category = "Electronics" },
        new { Id = 2, Name = "Smartphone", Price = 699.99, Category = "Electronics" },
        new { Id = 3, Name = "Headphones", Price = 149.99, Category = "Audio" },
        new { Id = 4, Name = "Keyboard", Price = 79.99, Category = "Electronics" },
        new { Id = 5, Name = "Mouse", Price = 49.99, Category = "Electronics" },
        new { Id = 6, Name = "Speaker", Price = 199.99, Category = "Audio" }
    };
    var filtered = string.IsNullOrEmpty(category) 
        ? allProducts 
        : allProducts.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(new { Category = category ?? "All", Products = filtered.ToList(), Timestamp = DateTime.UtcNow });
})
.WithName("GetProductsByCategory")
.WithTags("Products")
.WithMetadata(new ResponseCacheAttribute { Duration = 120, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "category" } });

// 3. Client-Side Caching Only - Browser caches, proxies don't
app.MapGet("/api/user-profile", () =>
{
    return Results.Ok(new { UserId = 1, Username = "john_doe", Email = "john@example.com", Timestamp = DateTime.UtcNow });
})
.WithName("GetUserProfile")
.WithTags("User")
.WithMetadata(new ResponseCacheAttribute { Duration = 300, Location = ResponseCacheLocation.Client });

// 4. No Caching - Security sensitive endpoints (login, private data)
app.MapPost("/api/login", (HttpRequest request) =>
{
    // Read request body manually for demo purposes
    return Results.Ok(new { Message = "Login endpoint - disable caching for security", Timestamp = DateTime.UtcNow });
})
.WithName("Login")
.WithTags("Auth")
.WithMetadata(new ResponseCacheAttribute { Duration = 0, Location = ResponseCacheLocation.None, NoStore = true });

app.MapGet("/api/private-data", () => Results.Ok(new { Message = "This is private data", Timestamp = DateTime.UtcNow }))
.WithName("GetPrivateData")
.WithTags("Data")
.WithMetadata(new ResponseCacheAttribute { Duration = 0, Location = ResponseCacheLocation.None, NoStore = true });

// 5. Cache with Header Variation - Different cache based on Accept header
app.MapGet("/api/time", () => Results.Ok(new { CurrentTime = DateTime.UtcNow, TimeZone = "UTC", Formatted = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }))
.WithName("GetCurrentTime")
.WithTags("Time")
.WithMetadata(new ResponseCacheAttribute { Duration = 10, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept" });

// 6. No Caching - Dynamic content that changes on each request
app.MapGet("/api/random-number", () => Results.Ok(new { Number = new Random().Next(1, 1000), Timestamp = DateTime.UtcNow }))
.WithName("GetRandomNumber")
.WithTags("Utilities")
.WithMetadata(new ResponseCacheAttribute { Duration = 0, Location = ResponseCacheLocation.None, NoStore = true });

// 7. Cache Status Endpoint - Check caching headers
app.MapGet("/api/cache-status", (HttpContext context) => Results.Ok(new
{
    CacheEnabled = true,
    CacheControl = context.Response.Headers.CacheControl.ToString(),
    Vary = context.Response.Headers.Vary.ToString(),
    Age = context.Response.Headers.Age.ToString(),
    ServerTimestamp = DateTime.UtcNow
}))
.WithName("GetCacheStatus")
.WithTags("Diagnostics");

Console.WriteLine("===========================================");
Console.WriteLine("ASP.NET Core Response Caching Sample");
Console.WriteLine("===========================================");
Console.WriteLine("Visit: http://localhost:5000/swagger");
Console.WriteLine("===========================================");

app.Run();