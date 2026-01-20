using FluentValidation;
using Shared.Dtos.Email;

namespace Services.Validation
{
    public class EmailAnhangValidator: AbstractValidator<EmailAnhangDto>
    {
        public EmailAnhangValidator() 
        {
            RuleFor(a => a.Dateiname)
            .NotEmpty().WithMessage("Der Dateiname darf nicht leer sein.")
            .MaximumLength(255).WithMessage("Der Dateiname ist zu lang.");

            RuleFor(a => a.Inhalt)
            .NotNull().WithMessage("Die Datei darf nicht null sein.")
            .Must(b => b.Length > 0).WithMessage("Die Datei darf nicht leer sein.")
            .Must(b => b.Length <= 10_000_000).WithMessage("Die Datei darf maximal 10 MB groß sein.");

        }
    }
}
