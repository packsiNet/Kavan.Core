using Kavan.Worker;
using InfrastructureLayer;
using ApplicationLayer.Interfaces.Services;
using Kavan.Worker.Services;
using Microsoft.AspNetCore.Hosting;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Register No-op broadcaster for Worker
builder.Services.AddSingleton<IKlineStreamBroadcaster, NoOpKlineStreamBroadcaster>();

// Register Mock WebHostEnvironment
builder.Services.AddSingleton<IWebHostEnvironment, WorkerHostEnvironment>();

// Register Infrastructure services (Database, Repositories, Background Services, etc.)
builder.Services.Register(builder.Configuration, isWorker: true);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<CandleAggregationWorker>();

var host = builder.Build();

// Apply migrations
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

host.Run();
