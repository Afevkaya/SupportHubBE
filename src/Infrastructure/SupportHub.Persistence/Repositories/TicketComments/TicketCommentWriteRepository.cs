using Persistence.Contexts;
using SupportHub.Application.Abstractions.Repositories.TicketComments;
using SupportHub.Domain.Entities;

namespace Persistence.Repositories.TicketComments;

public class TicketCommentWriteRepository(SupportHubDbContext context) : ITicketCommentWriteRepository
{
    public async Task<TicketComment> CreateAsync(TicketComment ticketComment)
    {
        var entityEntry = await context.TicketComments.AddAsync(ticketComment);
        await context.SaveChangesAsync();
        return entityEntry.Entity;
    }
}