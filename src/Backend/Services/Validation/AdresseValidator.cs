using FluentValidation;
using Shared.Dtos;
using System.Text.RegularExpressions;

namespace Services.Validation
{
    public class AdresseValidator: AbstractValidator<AdresseDto>
    {
        public AdresseValidator() 
        {
            RuleFor(a => a.Strasse).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Keine {PropertyName} angegeben").
                Length(2, 50).WithMessage("Ungültige Länge ({TotalLength}) für {PropertyName}").
                Must(name => Regex.IsMatch(name, @"^[\p{L}\d\s\-\.']+$")).WithMessage("Ungültige Eingabe für {PropertyName}");
            RuleFor(a => a.Stadt).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Keine {PropertyName} angegeben").
                Length(2, 50).WithMessage("Ungültige Länge ({TotalLength}) für {PropertyName}").
                Must(name => Regex.IsMatch(name, @"^[\p{L}\d\s\-\.']+$")).WithMessage("Ungültige Eingabe für {PropertyName}");
            RuleFor(a => a.Land).
                IsInEnum().WithMessage("Ungültiges Land.");
            //RuleFor(a => a.Land).
            //    Cascade(CascadeMode.Stop).
            //    NotEmpty().WithMessage("Kein {PropertyName} angegeben").
            //    Length(2, 50).WithMessage("Ungültige Länge ({TotalLength}) für {PropertyName}").
            //    Must(name => Regex.IsMatch(name, @"^[\p{L}\d\s\-\.']+$")).WithMessage("Ungültige Eingabe für {PropertyName}");
            RuleFor(a => a.PLZ).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Keine {PropertyName} angegeben").
                Matches(@"^\d{5}$").WithMessage("Die {PropertyName} muss aus genau 5 Ziffern bestehen");
            RuleFor(a => a.Hausnummer).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage("Keine {PropertyName} angegeben").
                Matches(@"^\d+$").WithMessage("Die {PropertyName} darf nur aus Ziffern bestehen");
            RuleFor(a => a.Hausnummerzusatz).
                Cascade(CascadeMode.Stop).
                MaximumLength(1).WithMessage("Ungültige Länge ({TotalLength}) für {PropertyName}").
                Must(z => string.IsNullOrEmpty(z) || z.All(char.IsLetter)).WithMessage("Ungültige Eingabe für {PropertyName}");

        } 
    }
}
