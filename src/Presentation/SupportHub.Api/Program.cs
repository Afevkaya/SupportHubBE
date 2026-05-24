using Persistence.Contexts;
using Persistence.Extensions;
using Serilog;
using Serilog.Events;
using SupportHub.Api.Middlewares;
using SupportHub.Application.Extensions;
using SupportHub.Infrastructure.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting SupportHub API...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen();
    builder.Services.AddApplicationServices();
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddInfrastructureServices();
    builder.Services.AddMemoryCache();
    builder.Services.AddHealthChecks()
        .AddNpgSql(builder.Configuration.GetConnectionString("PostgresSql") ?? "", name: "PostgreSQL", tags: ["db", "sql", "postgres"
        ]);
    

    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    {
        var env = hostingContext.HostingEnvironment;

        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", env.EnvironmentName)
            .Enrich.WithProperty("Application", "SupportHub.Api")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [CorrelationId:{CorrelationId}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [CorrelationId:{CorrelationId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30);

        if (env.IsDevelopment())
        {
            loggerConfiguration.MinimumLevel.Debug();
        }
    });

    var app = builder.Build();

    Log.Information("Creating HTTP request pipeline...");
    Log.Information("Environment: {EnvironmentName}", app.Environment.EnvironmentName); 
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapHealthChecks("/health");
    app.MapControllers();

    Log.Information("SupportHub API started successfully.");
    await app.RunAsync();
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
