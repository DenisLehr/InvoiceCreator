using FluentValidation;
using Shared.Domain.Enums;
using Shared.Dtos;

namespace Services.Validation
{
    public class KundeValidator: AbstractValidator<KundeDto>
    {
        public KundeValidator()
        {
            RuleFor(k => k.Vorname)
            .NotEmpty().WithMessage("Vorname ist erforderlich.")
            .MaximumLength(50)
            .Matches(@"^[a-zA-ZäöüÄÖÜß\s\-]+$").WithMessage("Nur Buchstaben, Leerzeichen und Bindestriche erlaubt.");

            RuleFor(k => k.Nachname)
                .NotEmpty().WithMessage("Nachname ist erforderlich.")
                .MaximumLength(50)
                .Matches(@"^[a-zA-ZäöüÄÖÜß\s\-]+$").WithMessage("Nur Buchstaben, Leerzeichen und Bindestriche erlaubt.");

            RuleFor(k => k.Firmenname)
                .Matches(@"^[a-zA-Z0-9äöüÄÖÜß&.,\-()'""\s]{2,100}$")
                    .When(x => !string.IsNullOrWhiteSpace(x.Firmenname))
                    .WithMessage("Der Firmenname darf nur Buchstaben, Zahlen und bestimmte Sonderzeichen enthalten.")
                .Length(2, 100)
                    .When(x => !string.IsNullOrWhiteSpace(x.Firmenname))
                    .WithMessage("Der Firmenname muss zwischen 2 und 100 Zeichen lang sein.");

            RuleFor(k => k.Email)
                .NotEmpty().WithMessage("E-Mail ist erforderlich.")
                .EmailAddress().WithMessage("Bitte eine gültige E-Mail-Adresse eingeben.")
                .MaximumLength(100);

            RuleFor(k => k.Telefon)
                .NotEmpty().WithMessage("Telefon ist erforderlich.")
                .Matches(@"^\+?[0-9\s\-()]{6,}$").WithMessage("Bitte eine gültige Telefonnummer eingeben.");

            RuleFor(k => k.TelefonMobil)
                .Matches(@"^\+?[0-9\s\-()]{6,}$").WithMessage("Bitte eine gültige Telefonnummer eingeben.");

            RuleFor(k => k.Geburtsdatum)
                .NotEmpty().WithMessage("Geburtsdatum ist erforderlich.")
                .Must(d => d <= DateTime.Today).WithMessage("Geburtsdatum darf nicht in der Zukunft liegen.");
            RuleFor(k => k.Geschlecht).
                IsInEnum().WithMessage("Ungültiges Geschlecht.");
            //RuleFor(k => k.Geschlecht).
            //    Must(value => Enum.TryParse<Geschlecht>(value, ignoreCase: true, out _)).
            //    WithMessage("Ungültiger Geschlechtswert.");


            RuleFor(k => k.Adresse)
                .NotNull().WithMessage("Adresse darf nicht null sein.")
                .SetValidator(new AdresseValidator());

            RuleFor(k => k.KundenRabatt)
                .InclusiveBetween(0, 100)
                .WithMessage("Kundenrabatt muss zwischen 0 und 100 liegen.");

        }

    }
}
