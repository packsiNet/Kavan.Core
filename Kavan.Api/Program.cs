using ApplicationLayer.Interfaces;
using AspNetCoreRateLimit;
using InfrastructureLayer;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Register(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add SignalR
builder.Services.AddSignalR();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

// Background hosted service for signals
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/Identity/swagger.json", "API Identity v1");
        options.SwaggerEndpoint("/swagger/External/swagger.json", "API External v1");
        options.SwaggerEndpoint("/swagger/Admin/swagger.json", "API Admin v1");
        options.SwaggerEndpoint("/swagger/Trader/swagger.json", "API Trader v1");
        options.SwaggerEndpoint("/swagger/Public/swagger.json", "API Public v1");
    });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        var seedService = services.GetRequiredService<ICandleSeedService>();
        seedService.SeedSymbolAsync().GetAwaiter().GetResult();

        var roleSeed = services.GetRequiredService<IRoleSeedService>();
        roleSeed.SeedRolesAsync().GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = services.GetService<ILoggerFactory>()?.CreateLogger("Startup");
        logger?.LogError(ex, "Startup seeding failed");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseIpRateLimiting();
app.UseClientRateLimiting();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();