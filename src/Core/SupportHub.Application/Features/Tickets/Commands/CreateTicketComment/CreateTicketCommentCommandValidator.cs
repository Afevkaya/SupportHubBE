using FluentValidation;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public class CreateTicketCommentCommandValidator : AbstractValidator<CreateTicketCommentCommand>
{
    public CreateTicketCommentCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Ticket ID zorunludur");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Yorum boş olamaz")
            .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir");
        
        RuleFor(x => x.Message)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.Trim().Length > 0)
            .WithMessage("Yorum sadece boşluklardan oluşamaz");
        
        RuleFor(x => x.Message)
            .Must(x => string.IsNullOrWhiteSpace(x) || !x.Contains("<script>", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Yorumda <script> etiketi kullanılamaz");
    }
}