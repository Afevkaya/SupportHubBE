using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.Constants;
using SupportHub.Application.Exceptions;
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
            roles = await userManager.GetRolesAsync(user);
        

        GetAllTicketsQueryResponse response;

        var currentUserId = currentService.UserId ??
                            throw new UnauthorizedAccessException("Kullanıcı bilgisi bulunamadı");
        
        var isAdmin = roles.Contains(Roles.Admin);
        var isCustomer = roles.Contains(Roles.Customer);
        var isSupportAgent = roles.Contains(Roles.SupportAgent);
        
        Guid? createdByUserId = null;
        Guid? assignedAgentId = null;
        
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
            assignedAgentId = currentUserId;
        }
        else
        {
            throw new ForbiddenAccessException("Tüm bilet listesini görüntüleme yetkiniz yok");
        }

        response = await ticketReadRepository.GetAllAsync(
            page: request.Page,
            pageSize: request.PageSize,
            createdByUserId: createdByUserId,
            assignedAgentId: assignedAgentId,
            sortBy: request.SortBy,
            sortDirection: request.SortDirection,
            status: request.Status,
            priority: request.Priority,
            search: request.Search);

        logger.LogInformation(
            "Tickets retrieved. Properties: UserId: {UserId}, Roles: {Roles}, IsCustomerScoped: {IsCustomerScoped}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, SortDirection: {SortDirection}, Status: {Status}, Search: {Search}, Priority: {Priority} TotalCount: {TotalCount}, ReturnedItemCount: {ReturnedItemCount}",
            currentService.UserId,
            string.Join(", ", roles),
            currentService.UserId != null && !roles.Contains(Roles.Admin) && !roles.Contains(Roles.SupportAgent),
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