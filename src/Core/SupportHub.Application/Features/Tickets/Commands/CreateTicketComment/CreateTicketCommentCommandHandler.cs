using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.TicketActivities;
using SupportHub.Application.Abstractions.Repositories.TicketComments;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Transactions;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public class CreateTicketCommentCommandHandler(
    ITicketReadRepository ticketReadRepository,
    ITicketCommentWriteRepository ticketCommentWriteRepository,
    ITicketActivityWriteRepository ticketActivityWriteRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateTicketCommentCommandHandler> logger) : IRequestHandler<CreateTicketCommentCommand, CreateTicketCommentCommandResponse>
{    

    public async Task<CreateTicketCommentCommandResponse> Handle(CreateTicketCommentCommand request, CancellationToken cancellationToken)
    {
        var exists = await ticketReadRepository.GetByIdAsync(request.TicketId);
        if (!exists)
            throw new KeyNotFoundException("Ticket not found");
        
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var ticketComment = new TicketComment
            {
                TicketId = request.TicketId,
                AuthorName = request.AuthorName,
                Message = request.Message,
                CreatedDate = DateTime.UtcNow
            };

            var response = await ticketCommentWriteRepository.CreateAsync(ticketComment);

            await ticketActivityWriteRepository.CreateAsync(new TicketActivity
            {
                Id = Guid.NewGuid(),
                TicketId = request.TicketId,
                ActivityType = Domain.Enums.TicketActivityType.CommentAdded,
                Description = $"Comment added by {request.AuthorName}",
                CreatedDate = DateTime.UtcNow
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Created comment for ticket {TicketId} by {AuthorName}", request.TicketId, request.AuthorName);
            return new CreateTicketCommentCommandResponse(response.Id, response.TicketId, response.AuthorName,
                response.Message, response.CreatedDate);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
