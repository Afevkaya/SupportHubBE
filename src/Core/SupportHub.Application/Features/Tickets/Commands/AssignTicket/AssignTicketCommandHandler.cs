using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.Constants;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Entities.Identity;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Features.Tickets.Commands.AssignTicket;

public class AssignTicketCommandHandler(
    ITicketReadRepository ticketReadRepository,
    ITicketWriteRepository ticketWriteRepository,
    ITicketActivityWriteRepository ticketActivityWriteRepository,
    UserManager<AppUser> userManager,
    ICacheService cacheService,
    ICurrentService currentService,
    ILogger<AssignTicketCommandHandler> logger)
    : IRequestHandler<AssignTicketCommand, AssignTicketCommandResponse>
{
    public async Task<AssignTicketCommandResponse> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketReadRepository.GetTicketAsync(request.TicketId);
        if (ticket is null)
        {
            logger.LogWarning("Attempted to assign non-existent ticket with ID {TicketId}", request.TicketId);
            throw new KeyNotFoundException("Ticket bulunamadı");
        }

        var agent = await userManager.FindByIdAsync(request.AssignedAgentId.ToString());
        if (agent == null)
        {
            logger.LogWarning("Attempted to assign ticket {TicketId} to non-existent user with ID {UserId}", request.TicketId, request.AssignedAgentId);
            throw new KeyNotFoundException("Kullanıcı bulunamadı");
        }

        var agentRole = await userManager.GetRolesAsync(agent);
        if (!agentRole.Contains(Roles.SupportAgent))
        {
            logger.LogWarning("Attempted to assign ticket {TicketId} to user {UserId} who is not a support agent", request.TicketId, request.AssignedAgentId);
            throw new InvalidOperationException("Kullanıcı bir destek temsilcisi olmalıdır");
        }

        var oldAssignedAgentId = ticket.AssignedAgentId;
        var updatedTicket = await ticketWriteRepository.AssignTicketToUserAsync(request.TicketId, request.AssignedAgentId);

        await ticketActivityWriteRepository.CreateAsync(new TicketActivity
        {
            Id = Guid.NewGuid(),
            TicketId = updatedTicket.Id,
            ActorUserId = currentService.UserId,
            ActivityType = TicketActivityType.Assigned,
            Description = oldAssignedAgentId.HasValue
                ? $"Ticket reassigned from agent {oldAssignedAgentId} to agent {updatedTicket.AssignedAgentId}"
                : $"Ticket assigned to agent {updatedTicket.AssignedAgentId}",
            CreatedDate = DateTime.UtcNow
        });

        await cacheService.RemoveByPrefixAsync("tickets_", cancellationToken);

        logger.LogInformation("Ticket {TicketId} reassigned from agent {OldAssignedAgentId} to agent {NewAssignedAgentId}", ticket.Id, oldAssignedAgentId, updatedTicket.AssignedAgentId);
        return new AssignTicketCommandResponse(updatedTicket.Id, updatedTicket.AssignedAgentId!.Value, "Bilet başarıyla atandı");
    }
}
