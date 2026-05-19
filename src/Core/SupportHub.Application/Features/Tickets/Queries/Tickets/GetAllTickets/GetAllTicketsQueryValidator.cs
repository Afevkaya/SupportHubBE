using FluentValidation;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public class GetAllTicketsQueryValidator : AbstractValidator<GetAllTicketsQuery>
{
    public GetAllTicketsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Sayfa numarası 1 veya daha büyük olmalıdır.");
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage("Sayfa boyutu 1 veya daha büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu en fazla 100 olabilir.");
    }
}