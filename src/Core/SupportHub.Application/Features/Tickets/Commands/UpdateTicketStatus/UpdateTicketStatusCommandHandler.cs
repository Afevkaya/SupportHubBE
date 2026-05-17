using MediatR;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler(ITicketWriteRepository ticketWriteRepository) : IRequestHandler<UpdateTicketStatusCommand, ResponseUpdateTicketStatus>
{
    public async Task<ResponseUpdateTicketStatus> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if(!Enum.IsDefined(request.Status))
            throw new ArgumentException("Invalid status value.");
        var ticket = await ticketWriteRepository.UpdateStatusAsync(request.Id, request.Status);
        var response = new ResponseUpdateTicketStatus(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Status.ToString(),
            ticket.CreatedDate,
            ticket.UpdatedDate
        );
        return response;
    }
}