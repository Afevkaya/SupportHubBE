using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.TicketComments;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Responses.Auths;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public class CreateTicketCommentCommandHandler(
    ITicketReadRepository ticketReadRepository,
    ITicketCommentWriteRepository ticketCommentWriteRepository,
    ITicketActivityWriteRepository ticketActivityWriteRepository,
    ICacheService cacheService,
    ICurrentService currentService,
    ILogger<CreateTicketCommentCommandHandler> logger) : IRequestHandler<CreateTicketCommentCommand, CreateTicketCommentCommandResponse>
{    

    public async Task<CreateTicketCommentCommandResponse> Handle(CreateTicketCommentCommand request, CancellationToken cancellationToken)
    {
        var exists = await ticketReadRepository.GetByIdAsync(request.TicketId);
        if (!exists)
            throw new KeyNotFoundException("Ticket not found");
        
        var ticketComment = new TicketComment
        {
            TicketId = request.TicketId,
            AuthorUserId = currentService.UserId,
            Message = request.Message,
            CreatedDate = DateTime.UtcNow
        };

        var response = await ticketCommentWriteRepository.CreateAsync(ticketComment);

        await ticketActivityWriteRepository.CreateAsync(new TicketActivity
        {
            Id = Guid.NewGuid(),
            TicketId = request.TicketId,
            ActorUserId = currentService.UserId,
            ActivityType = Domain.Enums.TicketActivityType.CommentAdded,
            Description = $"Comment added by {request.AuthorName}",
            CreatedDate = DateTime.UtcNow
        });
        
        await cacheService.RemoveByPrefixAsync("tickets_", cancellationToken);
        
        logger.LogInformation("Created comment for ticket {TicketId} by {UserId}", request.TicketId, currentService?.UserId);
        return new CreateTicketCommentCommandResponse(response.Id, response.TicketId,
            response.Message, response.CreatedDate, new ResponseGetAuth(currentService?.UserId, currentService?.FullName));
    }
}
