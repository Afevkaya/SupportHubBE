using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories.TicketComments;
using Persistence.Repositories.Tickets;
using SupportHub.Application.Abstractions.Repositories.TicketComments;
using SupportHub.Application.Abstractions.Repositories.Tickets;

namespace Persistence.Extensions;

public static class ServiceRegistrationExtension
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SupportHubDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresSql")));
        services.AddScoped<ITicketReadRepository, TicketReadRepository>();
        services.AddScoped<ITicketWriteRepository, TicketWriteRepository>();
        services.AddScoped<ITicketCommentWriteRepository, TicketCommentWriteRepository>();
    }
}