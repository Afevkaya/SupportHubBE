using MediatR;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler(ITicketWriteRepository ticketWriteRepository) : IRequestHandler<CreateTicketCommand, ResponseCreateTicket>
{
    public async Task<ResponseCreateTicket> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatusType.Open,
            CreatedDate = DateTime.UtcNow
        };
        var result = await ticketWriteRepository.CreateAsync(ticket);
        var response = new ResponseCreateTicket(
            result.Id,
            result.Title,
            result.Description,
            result.Status.ToString(),
            result.CreatedDate
        );
        return response;
    }
}