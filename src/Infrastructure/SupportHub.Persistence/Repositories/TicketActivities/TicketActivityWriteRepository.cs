using Persistence.Contexts;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace Persistence.Repositories.TicketActivities;

public class TicketActivityWriteRepository(SupportHubDbContext context) : ITicketActivityWriteRepository
{
    public Task CreateAsync(TicketActivity entity)
    {
        context.TicketActivities.Add(entity);
        return Task.CompletedTask;
    }
}
