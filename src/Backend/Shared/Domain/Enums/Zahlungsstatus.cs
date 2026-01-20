using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Zahlungsstatus
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
