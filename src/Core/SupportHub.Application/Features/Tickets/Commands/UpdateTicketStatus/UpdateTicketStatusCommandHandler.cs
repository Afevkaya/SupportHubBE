using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Application.DTOs.Responses.Auths;
using SupportHub.Application.DTOs.Responses.Tickets;

namespace SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler(
    ITicketWriteRepository ticketWriteRepository,
    ITicketActivityWriteRepository ticketActivityWriteRepository,
    ICacheService cacheService,
    ICurrentService currentService,
    ILogger<UpdateTicketStatusCommandHandler> logger) : IRequestHandler<UpdateTicketStatusCommand, ResponseUpdateTicketStatus>
{
    public async Task<ResponseUpdateTicketStatus> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if(!Enum.IsDefined(request.Status))
            throw new ArgumentException("Invalid status value.");

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
