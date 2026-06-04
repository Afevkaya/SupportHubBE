using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.TicketComments;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.Constants;
using SupportHub.Application.DTOs.Responses.Auths;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public class CreateTicketCommentCommandHandler(
    ITicketReadRepository ticketReadRepository,
    ITicketCommentWriteRepository ticketCommentWriteRepository,
    ITicketActivityWriteRepository ticketActivityWriteRepository,
    ICurrentService currentService,
    UserManager<AppUser> userManager,
    ILogger<CreateTicketCommentCommandHandler> logger) : IRequestHandler<CreateTicketCommentCommand, CreateTicketCommentCommandResponse>
{    

    public async Task<CreateTicketCommentCommandResponse> Handle(CreateTicketCommentCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketReadRepository.GetTicketAsync(request.TicketId);
        if (ticket == null)
            throw new KeyNotFoundException("Ticket bulunamadı");
        
        var currentUserId = currentService.UserId
                            ?? throw new UnauthorizedAccessException("Kullanıcı bilgisi bulunamadı");

        var currentUser = await userManager.FindByIdAsync(currentUserId.ToString());
        var roles = currentUser is null
            ? []
            : await userManager.GetRolesAsync(currentUser);

        var canCreateComment =
            roles.Contains(Roles.Admin) ||
            roles.Contains(Roles.SupportAgent) ||
            currentUserId == ticket.CreatedByUserId ||
            currentUserId == ticket.AssignedAgentId;

        if (!canCreateComment)
            throw new UnauthorizedAccessException("Bu bileti yorumlamak için yetkiniz yok");
        
        var ticketComment = new TicketComment
        {
            Id = Guid.NewGuid(),
            TicketId = request.TicketId,
            AuthorUserId = currentUserId,
            Message = request.Message,
            CreatedDate = DateTime.UtcNow
        };

        var response = await ticketCommentWriteRepository.CreateAsync(ticketComment);

        await ticketActivityWriteRepository.CreateAsync(new TicketActivity
        {
            Id = Guid.NewGuid(),
            TicketId = request.TicketId,
            ActorUserId = currentUserId,
            ActivityType = Domain.Enums.TicketActivityType.CommentAdded,
            Description = $"Comment added by {currentService.FullName}",
            CreatedDate = DateTime.UtcNow
        });
        
        logger.LogInformation("Created comment for ticket {TicketId} by {UserId}", request.TicketId, currentUserId);
        return new CreateTicketCommentCommandResponse(response.Id, response.TicketId,
            response.Message, response.CreatedDate, new ResponseGetAuth(currentUserId, currentService.FullName));
    }
}
