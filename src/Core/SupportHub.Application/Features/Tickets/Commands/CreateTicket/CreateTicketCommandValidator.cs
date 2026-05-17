using FluentValidation;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title boş olamaz")
            .MinimumLength(3).WithMessage("Title en az 3 karakter uzunluğunda olmalıdır.")
            .MaximumLength(200).WithMessage("Title en fazla 200 karakter uzunluğunda olabilir.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description boş olamaz")
            .MinimumLength(10).WithMessage("Description en az 10 karakter uzunluğunda olmalıdır.")
            .MaximumLength(2000).WithMessage("Description en fazla 2000 karakter uzunluğunda olabilir.");
    }
}