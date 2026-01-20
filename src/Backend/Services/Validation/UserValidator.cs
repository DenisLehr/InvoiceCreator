using FluentValidation;
using Shared.Domain.Enums;
using Shared.Dtos;

namespace Services.Validation
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(u => u.Email).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Keine {PropertyName} angegeben").
                MaximumLength(50).WithMessage("Die maximale Länge ({TotalLength}) der {PropertyName} wurde überschritten").
                EmailAddress().WithMessage("Keine gültige {PropertyName}");
            RuleFor(u => u.UserName).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Der Benutzername darf nicht leer sein.").
                MaximumLength(50).WithMessage("Der Benutzername darf maximal 50 Zeichen enthalten.");
            RuleFor(u => u.Vorname)
                .NotEmpty().WithMessage("Vorname ist erforderlich.")
                .MaximumLength(50)
                .Matches(@"^[a-zA-ZäöüÄÖÜß\s\-]+$").WithMessage("Nur Buchstaben, Leerzeichen und Bindestriche erlaubt.");

            RuleFor(u => u.Nachname)
                .NotEmpty().WithMessage("Nachname ist erforderlich.")
                .MaximumLength(50)
                .Matches(@"^[a-zA-ZäöüÄÖÜß\s\-]+$").WithMessage("Nur Buchstaben, Leerzeichen und Bindestriche erlaubt.");
            RuleFor(u => u.Initialen).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Initialen dürfen nicht leer sein.").
                Matches(@"^[A-Z]{2}$").WithMessage("Initialen müssen aus 2 Großbuchstaben bestehen.");
            RuleFor(u => u.Rolle).
                IsInEnum().WithMessage("Ungültige Rolle.");
            //RuleFor(u => u.Rolle).
            //    Must(value => Enum.TryParse<Rolle>(value, ignoreCase: true, out _)).
            //    WithMessage("Ungültige Rolle.");
        }

    }
}
