using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.TicketComments;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.Constants;
using SupportHub.Application.DTOs.Responses.Auths;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketComments;

public class GetTicketCommentsQueryHandler(
    ITicketCommentReadRepository ticketCommentReadRepository,
    UserManager<AppUser>  userManager,
    ITicketReadRepository ticketReadRepository, 
    ICurrentService currentService,
    ILogger<GetTicketCommentsQueryHandler> logger) : IRequestHandler<GetTicketCommentsQuery, List<GetTicketCommentsQueryResponse>>
{
    public async Task<List<GetTicketCommentsQueryResponse>> Handle(GetTicketCommentsQuery request, CancellationToken cancellationToken)
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

        var canViewComments =
            roles.Contains(Roles.Admin) ||
            currentUserId == ticket.CreatedByUserId ||
            currentUserId == ticket.AssignedAgentId;
        
        if (!canViewComments)
            throw new UnauthorizedAccessException("Bu bileti görüntüleme yetkiniz yok");
        
        var comments = await ticketCommentReadRepository.GetCommentsByTicketIdAsync(request.TicketId, cancellationToken);

        var authorIds = comments
            .Select(c => c.AuthorUserId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var authors = new Dictionary<Guid, AppUser>();
        foreach (var authorId in authorIds)
        {
            var author = await userManager.FindByIdAsync(authorId.ToString());
            if (author is not null)
                authors[authorId] = author;
        }

        var response = comments.Select(c =>
        {
            authors.TryGetValue(c.AuthorUserId ?? Guid.Empty, out var author);
            var fullName = author is null ? null : $"{author.FirstName} {author.LastName}";

            return new GetTicketCommentsQueryResponse(
                c.Id,
                c.TicketId,
                c.Message,
                c.CreatedDate,
                new ResponseGetAuth(c.AuthorUserId, fullName));
        }).ToList();

        logger.LogInformation(
            "Ticket comments retrieved. TicketId: {TicketId}, UserId: {UserId}, CommentCount: {CommentCount}",
            request.TicketId,
            currentUserId,
            response.Count);

        return response;
    }
}
