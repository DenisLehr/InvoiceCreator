using FluentValidation;
using Shared.Dtos.Email;

namespace Services.Validation
{
    public class EmailValidator: AbstractValidator<EmailDto>
    {
        public EmailValidator()
        {
            RuleFor(e => e.EmpfaengerEmail)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Der Empfänger darf nicht leer sein.")
            .EmailAddress().WithMessage("Ungültige E-Mail-Adresse.");

            RuleFor(e => e.Betreff)
                .NotEmpty().WithMessage("Der Betreff darf nicht leer sein.")
                .MaximumLength(150).WithMessage("Der Betreff darf maximal 150 Zeichen enthalten.");

            RuleFor(e => e.Nachricht)
                .NotEmpty().WithMessage("Die Nachricht darf nicht leer sein.")
                .MaximumLength(10_000).WithMessage("Die Nachricht ist zu lang.");

            RuleForEach(e => e.Anhang)
                .SetValidator(new EmailAnhangValidator());
        }
    }
}
