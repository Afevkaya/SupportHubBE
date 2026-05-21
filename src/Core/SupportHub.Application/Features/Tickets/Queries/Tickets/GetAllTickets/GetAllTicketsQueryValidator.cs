using FluentValidation;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public class GetAllTicketsQueryValidator : AbstractValidator<GetAllTicketsQuery>
{
    public GetAllTicketsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Sayfa numarası 1 veya daha büyük olmalıdır.");
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage("Sayfa boyutu 1 veya daha büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu en fazla 100 olabilir.");
        RuleFor(x => x.SortBy).Must(sortBy => string.IsNullOrEmpty(sortBy) || new[] { "createdDate", "status", "title", "priority" }.Contains(sortBy))
            .WithMessage("Geçersiz sıralama alanı. Geçerli alanlar: createdDate, status, title, priority.");
        RuleFor(x => x.SortDirection).Must(sortDirection => string.IsNullOrEmpty(sortDirection) || new[] { "asc", "desc" }.Contains(sortDirection.ToLower()))
            .WithMessage("Geçersiz sıralama yönü. Geçerli yönler: asc, desc.");
        RuleFor(x => x.Status).Must(status => string.IsNullOrEmpty(status) || Enum.TryParse<TicketStatusType>(status, true, out _))
            .WithMessage("Geçersiz durum değeri. Geçerli değerler: Open, Closed, InProgress, WaitingForResponse.");
        RuleFor(x => x.Search).MaximumLength(100).WithMessage("Arama terimi en fazla 100 karakter olabilir.");
        RuleFor(x => x.Priority).Must(priority => string.IsNullOrEmpty(priority) || Enum.TryParse<TicketPriorityType>(priority, true, out _))
            .WithMessage("Geçersiz öncelik değeri. Geçerli değerler: Low, Medium, High, Critical.");
    }
}