using FluentValidation;
using Shared.Domain.Enums;
using Shared.Dtos;

namespace Services.Validation
{
    public class LeistungValidator : AbstractValidator<LeistungDto>
    {
        public LeistungValidator()
        {

            RuleFor(l => l.Bezeichnung)
                .NotEmpty().WithMessage("Die Bezeichnung ist erforderlich.")
                .MaximumLength(100).WithMessage("Die Bezeichnung darf maximal 100 Zeichen lang sein.")
                .Matches(@"^[a-zA-Z0-9äöüÄÖÜß\s,\.\-\/\(\):\+→]+$")
                .WithMessage("Die Bezeichnung enthält ungültige Zeichen.");
            RuleFor(l => l.Beschreibung)
                .MaximumLength(500).WithMessage("Die Beschreibung darf maximal 500 Zeichen lang sein.")
                .Matches(@"^[a-zA-Z0-9äöüÄÖÜß\s,\.\-\/\(\):\+→]*$")
                .WithMessage("Die Beschreibung enthält ungültige Zeichen.")
                .When(l => !string.IsNullOrWhiteSpace(l.Beschreibung));
            RuleFor(l => l.Code)
                .Matches(@"^S\d{3}$")
                .WithMessage("Der Code muss im Format S001, S002, ... vorliegen.")
                .When(x => !string.IsNullOrWhiteSpace(x.Code));
            RuleFor(l => l.Pauschalpreis).
                GreaterThanOrEqualTo(0).WithMessage("Der Betrag darf nicht negativ sein.").
                LessThanOrEqualTo(1_000_000_000).WithMessage("Der Betrag ist zu groß.").
                Must(b => Decimal.Round(b, 2) == b).WithMessage("Der Betrag darf maximal zwei Nachkommastellen haben.");
            RuleFor(l => l.PreisPro15Min).
                GreaterThanOrEqualTo(0).WithMessage("Der Betrag darf nicht negativ sein.").
                LessThanOrEqualTo(1_000_000_000).WithMessage("Der Betrag ist zu groß.").
                Must(b => Decimal.Round(b, 2) == b).WithMessage("Der Betrag darf maximal zwei Nachkommastellen haben.");
            RuleFor(l => l.Richtzeit).
                Must(t => t.TotalMinutes > 0).
                WithMessage("Die Richtzeit muss größer als 0 Minuten sein.");
            RuleFor(l => l.Pauschalgrenze).
                Must(t => t.TotalMinutes > 0).
                WithMessage("Die Pauschalgrenze muss größer als 0 Minuten sein.");
            RuleFor(l => l.Steuersatz).
                IsInEnum().WithMessage("Ungültiger Steuersatz.");
            //RuleFor(l => l.Steuersatz).
            //    Must(value => Enum.TryParse<Steuersatz>(value, ignoreCase: true, out _)).
            //    WithMessage("Ungültiger Steuersatz.");
            RuleFor(l => l.Zusatzlogik)
                .NotNull()
                .WithMessage("Zusatzlogik muss angegeben werden.")
                .When(l => l.HatZusatzlogik);
            RuleFor(l => l.Zusatzlogik.Typ).
                IsInEnum().WithMessage("Ungültige Zusatzlogik.");
            //RuleFor(l => l.Zusatzlogik!.Typ)
            //    .Must(value => Enum.TryParse<ZusatzLogikTyp>(value, true, out _))
            //    .WithMessage("Ungültige Zusatzlogik.")
            //    .When(l => l.HatZusatzlogik && l.Zusatzlogik != null);
            RuleFor(l => l.Zusatzlogik!.Grenze)
                .GreaterThanOrEqualTo(0).WithMessage("Der Betrag darf nicht negativ sein.")
                .LessThanOrEqualTo(1_000_000_000).WithMessage("Der Betrag ist zu groß.")
                .Must(b => Decimal.Round(b, 2) == b).WithMessage("Der Betrag darf maximal zwei Nachkommastellen haben.")
                .When(l => l.HatZusatzlogik && l.Zusatzlogik != null);
            RuleFor(l => l.Zusatzlogik!.PreisProEinheit)
                .GreaterThanOrEqualTo(0).WithMessage("Der Betrag darf nicht negativ sein.")
                .LessThanOrEqualTo(1_000_000_000).WithMessage("Der Betrag ist zu groß.")
                .Must(b => Decimal.Round(b, 2) == b).WithMessage("Der Betrag darf maximal zwei Nachkommastellen haben.")
                .When(l => l.HatZusatzlogik && l.Zusatzlogik != null);

        }

    }
}
