using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public class GetOpenTicketsQueryHandler(
    ITicketReadRepository ticketReadRepository, 
    ICurrentService currentService,
    UserManager<AppUser> userManager,
    ILogger<GetOpenTicketsQueryHandler> logger) : IRequestHandler<GetOpenTicketsQuery, GetOpenTicketsQueryResponse>
{
    public async Task<GetOpenTicketsQueryResponse> Handle(GetOpenTicketsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(currentService.UserId.ToString() ?? string.Empty);
        IList<string> roles = new List<string>();
        if (user != null)
        {
            roles = await userManager.GetRolesAsync(user);
        }
        
        GetOpenTicketsQueryResponse response;
        if (roles.Contains("Admin") || roles.Contains("Support"))
        {
            response = await ticketReadRepository.GetOpenTicketsAsync(request.Page, request.PageSize, null, request.SortBy, request.SortDirection, cancellationToken);
        }
        else
        {
            response = await ticketReadRepository.GetOpenTicketsAsync(request.Page, request.PageSize, currentService.UserId, request.SortBy, request.SortDirection, cancellationToken);
        }
        logger.LogInformation(
            "Open tickets retrieved with pagination. Properties: UserId: {UserId}, Roles: {Roles}, IsCustomerScoped: {IsCustomerScoped}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, SortDirection: {SortDirection}, TotalCount: {TotalCount}, ReturnedItemCount: {ReturnedItemCount}",
            currentService.UserId,
            string.Join(", ", roles),
            currentService.UserId != null && !roles.Contains("Admin") && !roles.Contains("Support"),
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            response.TotalCount,
            response.Items.Count);
        return response;
    }
}