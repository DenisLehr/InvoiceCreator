using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Terminstatus
    {
        [Display(Name ="Geplant")]
        Geplant,
        [Display(Name = "Bestätigt")]
        Bestaetigt,
        [Display(Name = "Abgesagt")]
        Abgesagt,
        [Display(Name = "Erledigt")]
        Erledigt
    }
}
