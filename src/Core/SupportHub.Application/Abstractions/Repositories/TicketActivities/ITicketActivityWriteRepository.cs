using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Abstractions.Repositories.TicketActivities;

public interface ITicketActivityWriteRepository
{
    Task CreateAsync(TicketActivity entity);
}