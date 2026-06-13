using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using SupportHub.Api.Authorization;
using SupportHub.Api.Models.Responses;
using SupportHub.Application.Authorization;

namespace SupportHub.Api.Extensions;

public static class ServiceRegistrationExtension
{
    public static void AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddSwaggerServices(services);
        ConfigureApiBehavior(services);
        AddPermissionPolicies(services);
        AddAuthenticationServices(services, configuration);
        AddSupportHubHealthChecks(services, configuration);
    }

    private static void AddSwaggerServices(IServiceCollection services)
        {
            services.AddOpenApi();
            services.AddSwaggerGen(options =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT token format: Bearer {token}"
                };
    
                options.AddSecurityDefinition("Bearer", securityScheme);
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document, externalResource: null),
                        []
                    }
                });
            });
        }
    private static void ConfigureApiBehavior(IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors
                            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "Invalid value."
                                : error.ErrorMessage)
                            .ToList());

                return new BadRequestObjectResult(new ValidationErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "Validation failed.",
                    errors));
            };
        });
    }
    private static void AddPermissionPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission, policy => policy.AddRequirements(new PermissionRequirement(permission)));
            }
        });
        
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }
    private static void AddAuthenticationServices(IServiceCollection services, IConfiguration configuration)
    {
        // JWT Authentication configuration
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Warning(context.Exception, "JWT authentication failed.");
                    return Task.CompletedTask;
                },
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new ErrorResponse(
                        StatusCodes.Status401Unauthorized,
                        "Authentication is required.");

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var response = new ErrorResponse(
                        StatusCodes.Status403Forbidden,
                        "You do not have permission to access this resource.");

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
                }
            };
        });
    }
    private static void AddSupportHubHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresSql") ?? "", name: "PostgreSQL", tags: ["db", "sql", "postgres"
            ]);
    }
}
