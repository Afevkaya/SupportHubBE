using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses.Tickets;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

public class GetTicketDetailQueryHandler(
    ITicketReadRepository ticketReadRepository,
    UserManager<AppUser> userManager,
    ILogger<GetTicketDetailQueryHandler> logger) : IRequestHandler<GetTicketDetailQuery, GetTicketDetailQueryResponse>
{
    public async Task<GetTicketDetailQueryResponse> Handle(GetTicketDetailQuery request, CancellationToken cancellationToken)
    {
        var ticket = await ticketReadRepository.GetTicketDetail(request.Id);
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
                        var authorName = c.AuthorUserId.HasValue && authorNames.TryGetValue(c.AuthorUserId.Value, out var name)
                            ? name
                            : null;
                        return new ResponseTicketComment(authorName, c.Message);
                    })
                    .ToList(),
                ticket.TicketActivities
                    .OrderBy(a => a.CreatedDate)
                    .Select(a => new ResponseTicketActivity(a.ActivityType.ToString(), a.Description, a.CreatedDate))
                    .ToList()
            );
        }

        logger.LogWarning("Ticket with id {Id} not found", request.Id);
        throw new KeyNotFoundException("Ticket not found");
    }
}
