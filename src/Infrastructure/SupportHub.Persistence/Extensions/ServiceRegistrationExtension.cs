using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories.TicketActivities;
using Persistence.Repositories.TicketComments;
using Persistence.Repositories.Tickets;
using Persistence.UnitOfWorks;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.TicketComments;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Transactions;
using SupportHub.Domain.Entities.Identity;

namespace Persistence.Extensions;

public static class ServiceRegistrationExtension
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SupportHubDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresSql")));
        services.AddIdentityCore<AppUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<SupportHubDbContext>();
        services.AddScoped<ITicketReadRepository, TicketReadRepository>();
        services.AddScoped<ITicketWriteRepository, TicketWriteRepository>();
        services.AddScoped<ITicketCommentWriteRepository, TicketCommentWriteRepository>();
        services.AddScoped<ITicketActivityWriteRepository, TicketActivityWriteRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
