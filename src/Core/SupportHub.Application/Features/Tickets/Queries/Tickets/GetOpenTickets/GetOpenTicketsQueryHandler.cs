using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.Constants;
using SupportHub.Application.Exceptions;
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
        var currentUserId = currentService.UserId
                            ?? throw new UnauthorizedAccessException("Kullanıcı bilgisi bulunamadı");

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
            throw new ForbiddenAccessException("Açık biletleri görüntüleme yetkiniz yok");
        }
        
        response = await ticketReadRepository.GetOpenTicketsAsync(
            request.Page,
            request.PageSize,
            createdByUserId,
            assignedAgentId,
            request.SortBy,
            request.SortDirection,
            cancellationToken);
        
        logger.LogInformation(
            "Open tickets retrieved with pagination. Properties: UserId: {UserId}, Roles: {Roles}, IsCustomerScoped: {IsCustomerScoped}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, SortDirection: {SortDirection}, TotalCount: {TotalCount}, ReturnedItemCount: {ReturnedItemCount}",
            currentService.UserId,
            string.Join(", ", roles),
            currentService.UserId != null && !roles.Contains(Roles.Admin) && !roles.Contains(Roles.SupportAgent),
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            response.TotalCount,
            response.Items.Count);
        return response;
    }
}