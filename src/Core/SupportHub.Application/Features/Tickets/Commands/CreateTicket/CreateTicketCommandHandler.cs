using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Transactions;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler(
    ITicketWriteRepository ticketWriteRepository, 
    ITicketActivityWriteRepository ticketActivityWriteRepository,
    ICacheService cacheService,
    ILogger<CreateTicketCommandHandler> logger) 
    : IRequestHandler<CreateTicketCommand, ResponseCreateTicket>
{
    public async Task<ResponseCreateTicket> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatusType.Open,
            Priority = request.Priority ?? TicketPriorityType.Medium,
            CreatedDate = DateTime.UtcNow
        };

        var result = await ticketWriteRepository.CreateAsync(ticket);

        await ticketActivityWriteRepository.CreateAsync(new TicketActivity
        {
            Id = Guid.NewGuid(),
            TicketId = result.Id,
            ActivityType = TicketActivityType.Created,
            Description = $"Ticket created with status {result.Status} and priority {result.Priority}",
            CreatedDate = DateTime.UtcNow
        });


        var response = new ResponseCreateTicket(
            result.Id,
            result.Title,
            result.Description,
            result.Status.ToString(),
            result.Priority.ToString(),
            result.CreatedDate
        );
        await cacheService.RemoveByPrefixAsync("tickets_", cancellationToken);
        logger.LogInformation("Ticket created with Id: {Id}, Status: {Status}, Priority: {Priority}", result.Id, result.Status, result.Priority);
        return response;
    
    }
}
