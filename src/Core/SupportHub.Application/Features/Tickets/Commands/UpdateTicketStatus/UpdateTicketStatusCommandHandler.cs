using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Transactions;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler(
    ITicketWriteRepository ticketWriteRepository,
    ITicketActivityWriteRepository ticketActivityWriteRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateTicketStatusCommandHandler> logger) : IRequestHandler<UpdateTicketStatusCommand, ResponseUpdateTicketStatus>
{
    public async Task<ResponseUpdateTicketStatus> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if(!Enum.IsDefined(request.Status))
            throw new ArgumentException("Invalid status value.");

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var ticket = await ticketWriteRepository.UpdateStatusAsync(request.Id, request.Status);

            await ticketActivityWriteRepository.CreateAsync(new Domain.Entities.TicketActivity
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                ActivityType = Domain.Enums.TicketActivityType.StatusChanged,
                Description = $"Ticket status changed to {ticket.Status}",
                CreatedDate = DateTime.UtcNow
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            var response = new ResponseUpdateTicketStatus(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status.ToString(),
                ticket.CreatedDate,
                ticket.UpdatedDate
            );

            logger.LogInformation("Updated status for ticket {TicketId} to {Status}", ticket.Id, ticket.Status);
            return response;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
