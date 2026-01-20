using FluentValidation;
using Shared.Dtos;

namespace Application.Common.Validators
{
    public class RechnungspostenValidator : AbstractValidator<RechnungspostenDto>
    {
        public RechnungspostenValidator()
        {
            RuleFor(r => r.LeistungID).NotEmpty();
            RuleFor(r => r.Bezeichnung).NotEmpty().MaximumLength(100);
            RuleFor(r => r.Menge).GreaterThan(0);
            RuleFor(r => r.Einzelpreis).GreaterThanOrEqualTo(0);
            RuleFor(r => r.Steuersatz).
                IsInEnum().WithMessage("Ungültiger Steuersatz.");
            //RuleFor(l => l.Steuersatz).
            //    Must(value => Enum.TryParse<Steuersatz>(value, ignoreCase: true, out _)).
            //    WithMessage("Ungültiger Steuersatz.");
            RuleFor(r => r.Rabatt).InclusiveBetween(0, 100);
            RuleFor(r => r.Einheit).IsInEnum();
        }
    }
}
