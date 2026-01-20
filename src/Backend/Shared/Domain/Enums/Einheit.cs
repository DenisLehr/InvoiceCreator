using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Einheit
    {
        [Display(Name = "Stunde")]
        HUR,
        [Display(Name = "Tag")]
        DAY,
        [Display(Name = "Monat")]
        MON,
        [Display(Name = "Dienstleistung")]
        E48,
        [Display(Name = "Stück")]
        C62,
        [Display(Name = "Pauschale")]
        LS,
        [Display(Name = "Benutzer")]
        IE
    }

}
