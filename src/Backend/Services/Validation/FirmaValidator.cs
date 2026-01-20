using FluentValidation;
using Shared.Domain.Enums;
using Shared.Dtos;
using System.Text.RegularExpressions;

namespace Services.Validation
{ 
    public class FirmaValidator: AbstractValidator<FirmaDto>
    {
        public FirmaValidator()
        {
            RuleFor(f => f.Email).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Keine {PropertyName} angegeben").
                MaximumLength(50).WithMessage("Die maximale Länge ({TotalLength}) der {PropertyName} wurde überschritten").
                EmailAddress().WithMessage("Keine gültige {PropertyName}");
            RuleFor(f => f.Telefon).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Keine {PropertyName} angegeben").
                Matches(@"^(\+49|0)[\d\s\-\/]{6,}$").WithMessage("Keine gültige deutsche Telefonnummer");
            RuleFor(f => f.Name).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Kein {PropertyName} angegeben").
                Length(2, 100).WithMessage("Ungültige Länge ({TotalLength}) für {PropertyName}").
                Must(name => Regex.IsMatch(name, @"^[\p{L}\s\-\.&]+$"));
            RuleFor(x => x.UStId).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Die Umsatzsteuer-ID darf nicht leer sein.").
                Matches(@"^[A-Z]{2}[0-9]{8,12}$").WithMessage("Ungültige Umsatzsteuer-ID");
            RuleFor(f => f.Adresse).
                SetValidator(new AdresseValidator());
            RuleFor(f => f.Bankverbindung).
                SetValidator(new BankverbindungValidator());
            RuleFor(f => f.Rechtsform).
                IsInEnum().WithMessage("Ungültige Rechtsform.");
            //RuleFor(x => x.Rechtsform)
            //    .Must(value => Enum.TryParse<Rechtsform>(value, true, out _))
            //    .WithMessage("Ungültige Rechtsform.")
            //    .When(f => !string.IsNullOrWhiteSpace(f.Rechtsform));
            RuleFor(f => f.HandelsregisterNr)
                .Cascade(CascadeMode.Stop)
                .Matches(@"^(HRB|HRA)\s?\d{1,8}$")
                .When(f => !string.IsNullOrWhiteSpace(f.HandelsregisterNr))
                .WithMessage("Handelsregister‑Nummer muss im Format HRB 12345 oder HRA 12345 sein.");

            RuleFor(f => f.Registergericht)
                .NotEmpty()
                .WithMessage("Registergericht ist erforderlich, wenn eine Handelsregister‑Nummer angegeben ist.")
                .When(f => !string.IsNullOrWhiteSpace(f.HandelsregisterNr));

            // Geschäftsführer: für Kapitalgesellschaften mindestens einer, sonst optional
            RuleFor(f => f.Geschaeftsfuehrer)
                .NotNull()
                .When(x => RequiresGeschaeftsfuehrer(x.Rechtsform))
                .WithMessage("Mindestens ein Geschäftsführer ist für diese Rechtsform erforderlich.")
                .DependentRules(() =>
                {
                    RuleForEach((FirmaDto firma) => firma.Geschaeftsfuehrer!)
                        .NotEmpty().WithMessage("Name des Geschäftsführers darf nicht leer sein.")
                        .MaximumLength(100).WithMessage("Name des Geschäftsführers darf max. 100 Zeichen haben.")
                        .Matches(@"^[\p{L}\p{M}'\-\.\s]+$").WithMessage("Name enthält ungültige Zeichen.");
                });
        
        }

        private bool RequiresGeschaeftsfuehrer(Rechtsform? rechtsform)
        {
            if (rechtsform is null) return false;

            switch (rechtsform)
            {
                case Rechtsform.GmbH:
                case Rechtsform.UG:
                case Rechtsform.GmbHCoKG:
                case Rechtsform.AG:
                    return true;
                default:
                    return false;
            }
        }

    }
}
