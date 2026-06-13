using Persistence.Extensions;
using Serilog;
using SupportHub.Application.Extensions;
using SupportHub.Infrastructure.Extensions;
using SupportHub.Api.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting SupportHub API...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSupportHubSerilog();
    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddApplicationServices();
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddInfrastructureServices();
    builder.Services.AddPresentationServices(builder.Configuration);
    builder.Services.AddMemoryCache();

    var app = builder.Build();
    await app.Services.SeedDefaultRolesAsync();

    Log.Information("Creating HTTP request pipeline...");
    Log.Information("Environment: {EnvironmentName}", app.Environment.EnvironmentName); 
    app.UseSupportHubMiddlewares();
    app.UseSwaggerDocumentation();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapSupportHubEndpoints();

    Log.Information("SupportHub API started successfully.");
    await app.RunAsync();
}
catch (HostAbortedException)
{
    // EF Core design-time tools can trigger this.
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("SupportHub API shutting down...");
    await Log.CloseAndFlushAsync();
}
