using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.Constants;
using SupportHub.Application.DTOs.Responses.Tickets;
using SupportHub.Application.Exceptions;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

public class GetTicketDetailQueryHandler(
    ITicketReadRepository ticketReadRepository,
    ICurrentService currentService,
    UserManager<AppUser> userManager,
    ILogger<GetTicketDetailQueryHandler> logger) : IRequestHandler<GetTicketDetailQuery, GetTicketDetailQueryResponse>
{
    public async Task<GetTicketDetailQueryResponse> Handle(GetTicketDetailQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentService.UserId
            ?? throw new UnauthorizedAccessException("Kullanıcı bilgisi bulunamadı.");

        var currentUser = await userManager.FindByIdAsync(currentUserId.ToString());
        var roles = currentUser is null
            ? []
            : await userManager.GetRolesAsync(currentUser);

        var isAdmin = roles.Contains(Roles.Admin);
        var isCustomer = roles.Contains(Roles.Customer);
        var isSupportAgent = roles.Contains(Roles.SupportAgent);
        
        if (!isAdmin && !isCustomer && !isSupportAgent)
            throw new ForbiddenAccessException("Bu bileti görüntüleme yetkiniz yok");
        
        
        Guid? createdByUserId = null;
        Guid? assignedToUserId = null;
        
        if (isAdmin)
        {
            // scope yok, tüm ticketlar
        }
        else if (isCustomer)
        {
            createdByUserId = currentUserId;
        }
        else if (isSupportAgent)
        {
            assignedToUserId = currentUserId;
        }
        else
        {
            throw new ForbiddenAccessException("Bu bileti görüntüleme yetkiniz yok");
        }

        var ticket = await ticketReadRepository.GetTicketDetail(request.Id, createdByUserId, assignedToUserId);
        if (ticket is not null)
        {
            var authorIds = ticket.TicketComments
                .Select(c => c.AuthorUserId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var authorNames = new Dictionary<Guid, string>();
            foreach (var authorId in authorIds)
            {
                var author = await userManager.FindByIdAsync(authorId.ToString());
                if (author is not null)
                {
                    authorNames[authorId] = $"{author.FirstName} {author.LastName}".Trim();
                }
            }

            return new GetTicketDetailQueryResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status.ToString(),
                ticket.Priority.ToString(),
                ticket.CreatedDate,
                ticket.UpdatedDate,
                ticket.TicketComments
                    .OrderBy(c => c.CreatedDate)
                    .Select(c =>
                    {
                        string? authorName = null;
                        if (c.AuthorUserId.HasValue && authorNames.TryGetValue(c.AuthorUserId.Value, out var name))
                        {
                            authorName = name;
                        }

                        return new ResponseTicketComment(authorName, c.Message);
                    })
                    .ToList(),
                ticket.TicketActivities
                    .OrderBy(a => a.CreatedDate)
                    .Select(a => new ResponseTicketActivity(a.ActivityType.ToString(), a.Description, a.CreatedDate))
                    .ToList()
            );
        }

        logger.LogWarning(
            "Ticket detail not found or not accessible. TicketId: {TicketId}, UserId: {UserId}",
            request.Id,
            currentUserId);
        throw new KeyNotFoundException("Bilet bulunamadı.");
    }
}
