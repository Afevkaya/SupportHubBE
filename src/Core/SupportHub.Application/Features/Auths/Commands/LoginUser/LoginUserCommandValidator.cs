using FluentValidation;

namespace SupportHub.Application.Features.Auths.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adres zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}