using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Responses.Auths;
using SupportHub.Application.DTOs.Responses.Tickets;
using SupportHub.Application.Exceptions;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler(
    ITicketWriteRepository ticketWriteRepository,
    ITicketActivityWriteRepository ticketActivityWriteRepository,
    ITicketReadRepository ticketReadRepository,
    UserManager<AppUser> userManager,
    ICacheService cacheService,
    ICurrentService currentService,
    ILogger<UpdateTicketStatusCommandHandler> logger) : IRequestHandler<UpdateTicketStatusCommand, ResponseUpdateTicketStatus>
{
    public async Task<ResponseUpdateTicketStatus> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if(!Enum.IsDefined(request.Status))
            throw new ArgumentException("Invalid status value.");

        var currentUserId = currentService.UserId
                            ?? throw new UnauthorizedAccessException("Kullanıcı bilgisi bulunamadı");
        
        
        var existingTicket = await ticketReadRepository.GetTicketAsync(request.Id);
        if (existingTicket == null)
            throw new KeyNotFoundException("Bilet bulunamadı");
        
        var currentUser = await userManager.FindByIdAsync(currentUserId.ToString());
        var roles = currentUser is null
            ? []
            : await userManager.GetRolesAsync(currentUser);
        
        var isAdmin = roles.Contains(Constants.Roles.Admin);
        var isSupportAgent = roles.Contains(Constants.Roles.SupportAgent);
        var isAssignedAgent = existingTicket.AssignedAgentId == currentUserId;

        var canUpdateStatus = isAdmin || (isSupportAgent && isAssignedAgent);
        
        if (!canUpdateStatus)
            throw new ForbiddenAccessException("Bu bileti güncellemeye yetkiniz yok.");

        var ticket = await ticketWriteRepository.UpdateStatusAsync(request.Id, request.Status);

        await ticketActivityWriteRepository.CreateAsync(new Domain.Entities.TicketActivity
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            ActorUserId = currentService.UserId,
            ActivityType = Domain.Enums.TicketActivityType.StatusChanged,
            Description = $"Ticket status changed to {ticket.Status}",
            CreatedDate = DateTime.UtcNow
        });

        var response = new ResponseUpdateTicketStatus(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Status.ToString(),
            ticket.CreatedDate,
            ticket.UpdatedDate,
            new ResponseGetAuth(currentService?.UserId, currentService?.FullName)
        );

        await cacheService.RemoveByPrefixAsync("tickets_", cancellationToken);
        
        logger.LogInformation("Updated status for ticket {TicketId} to {Status} by UserId: {UserId}", ticket.Id, ticket.Status, currentService?.UserId);
        return response;
    }
}
