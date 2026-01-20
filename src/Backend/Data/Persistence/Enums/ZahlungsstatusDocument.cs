using System.ComponentModel.DataAnnotations;

namespace Data.Persistence.Enums
{
    public enum ZahlungsstatusDocument
    {
        [Display(Name = "Offen")]
        Offen,

        [Display(Name = "Teilweise bezahlt")]
        TeilweiseBezahlt,

        [Display(Name = "Bezahlt")]
        Bezahlt,

        [Display(Name = "Überfällig")]
        Ueberfaellig,

        [Display(Name = "Storniert")]
        Storniert,

        [Display(Name = "Rückerstattet")]
        Rueckerstattet
    }
}
