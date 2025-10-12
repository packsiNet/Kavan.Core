using AspNetCoreRateLimit;
using InfrastructureLayer;

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