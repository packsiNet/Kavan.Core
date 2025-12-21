using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Kavan.Worker.Services;

public class WorkerHostEnvironment : IWebHostEnvironment
{
    public string WebRootPath { get; set; } = "wwwroot";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string ApplicationName { get; set; } = "Kavan.Worker";
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public string EnvironmentName { get; set; } = "Production";
}
