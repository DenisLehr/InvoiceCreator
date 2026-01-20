using FluentValidation;
using Shared.Dtos;

namespace Services.Validation
{
    public class BankverbindungValidator: AbstractValidator<BankverbindungDto>
    {
        public BankverbindungValidator()
        {
            RuleFor(b => b.Kontoinhaber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Der Kontoinhaber darf nicht leer sein.")
                .Length(2,50);
            RuleFor(b => b.IBAN)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Die IBAN darf nicht leer sein.")
                .Matches(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$")
                .WithMessage("Die IBAN ist nicht im gültigen Format.");
            RuleFor(b => b.BIC)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Die BIC darf nicht leer sein.")
                .Matches(@"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$")
                .WithMessage("Die BIC ist nicht im gültigen Format.");
            RuleFor(b => b.Bankname)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Der Bankname darf nicht leer sein.")
                .MaximumLength(100)
                .Matches(@"^[a-zA-Z0-9äöüÄÖÜß&.,\-()'""\s]{2,100}$")
                .WithMessage("Der Bankname darf nur Buchstaben, Zahlen und bestimmte Sonderzeichen enthalten.");
                
        }
    }
}
