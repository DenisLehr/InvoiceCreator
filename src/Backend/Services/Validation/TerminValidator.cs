using FluentValidation;
using Shared.Dtos;

namespace Services.Validation
{
    public class TerminValidator: AbstractValidator<TerminDto>
    {
        public TerminValidator()
        {
            RuleFor(t => t.Start)
                .LessThan(t => t.End)
                .WithMessage("Startzeit muss vor Endzeit liegen.");

            RuleFor(t => t.GeschätzteDauer)
                .NotNull()
                .WithMessage("Die Dauer muss berechnet werden.")
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("Die Dauer muss größer als 0 sein.");

            RuleFor(t => t.Leistungen)
                .NotEmpty()
                .WithMessage("Mindestens eine Leistung muss angegeben werden.");

            RuleFor(t => t.Text)
                .NotEmpty()
                .Must(t => !string.IsNullOrWhiteSpace(t))
                .WithMessage("Beschreibung darf nicht leer sein.");
        }
    }
}
