using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public class GetAllTicketsQueryHandler(
    ITicketReadRepository ticketReadRepository,
    UserManager<AppUser> userManager,
    ICurrentService currentService,
    ILogger<GetAllTicketsQueryHandler> logger) : IRequestHandler<GetAllTicketsQuery, GetAllTicketsQueryResponse>
{
    public async Task<GetAllTicketsQueryResponse> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(currentService.UserId.ToString() ?? string.Empty);
        IList<string> roles = new List<string>();
        if (user != null)
        {
            roles = await userManager.GetRolesAsync(user);
        }

        GetAllTicketsQueryResponse response;

        if (roles.Contains("Admin") || roles.Contains("Support"))
        {
            response = await ticketReadRepository.GetAllAsync(
                request.Page,
                request.PageSize,
                null,
                request.SortBy,
                request.SortDirection,
                request.Status,
                request.Priority,
                request.Search);
        }
        else
        {
            response = await ticketReadRepository.GetAllAsync(
                request.Page,
                request.PageSize,
                currentService.UserId,
                request.SortBy,
                request.SortDirection,
                request.Status,
                request.Priority,
                request.Search);
        }

        logger.LogInformation(
            "Tickets retrieved. Properties: UserId: {UserId}, Roles: {Roles}, IsCustomerScoped: {IsCustomerScoped}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, SortDirection: {SortDirection}, Status: {Status}, Search: {Search}, Priority: {Priority} TotalCount: {TotalCount}, ReturnedItemCount: {ReturnedItemCount}",
            currentService.UserId,
            string.Join(", ", roles),
            currentService.UserId != null && !roles.Contains("Admin") && !roles.Contains("Support"),
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            request.Status,
            request.Search,
            request.Priority,
            response.TotalCount,
            response.Items.Count);

        return response;
    }
}