using AspNetCoreRateLimit;
using InfrastructureLayer;
using Microsoft.OpenApi.Models;

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

// SwaggerGen از InfrastructureLayer پیکربندی شده است؛ در اینجا دوباره تعریف نمی‌کنیم

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // نمایش داک پیش‌فرض v1 برای جلوگیری از خطای 404
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "All APIs v1");
        // نمایش داک‌های گروهی مطابق تنظیمات InfrastructureLayer
        options.SwaggerEndpoint("/swagger/Users/swagger.json", "API Users v1");
        options.SwaggerEndpoint("/swagger/Identity/swagger.json", "API Identity v1");
        options.SwaggerEndpoint("/swagger/Administrator/swagger.json", "API Administrator v1");
        options.SwaggerEndpoint("/swagger/Managers/swagger.json", "API Managers v1");
        options.SwaggerEndpoint("/swagger/MiniApp/swagger.json", "API MiniApp v1");
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