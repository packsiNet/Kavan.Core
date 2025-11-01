using AspNetCoreRateLimit;
using InfrastructureLayer;
using ApplicationLayer.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Register(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add SignalR
builder.Services.AddSignalR();

builder.Services.AddOpenApi();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

var app = builder.Build();

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seedingService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedingService>();
        await seedingService.SeedCryptocurrenciesAsync();
        await seedingService.SeedTimeFramesAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/Users/swagger.json", "API Users v1");
        //options.SwaggerEndpoint("/swagger/Identity/swagger.json", "API Identity v1");
        //options.SwaggerEndpoint("/swagger/Administrator/swagger.json", "API Administrator v1");
        //options.SwaggerEndpoint("/swagger/Managers/swagger.json", "API Managers v1");
        //options.SwaggerEndpoint("/swagger/MiniApp/swagger.json", "API MiniApp v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseIpRateLimiting();
app.UseClientRateLimiting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();