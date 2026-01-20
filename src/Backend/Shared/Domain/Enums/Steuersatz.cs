using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Steuersatz
    {
        [Display(Name="0 %")]
        Keine = 0,
        [Display(Name = "7 %")]
        SiebenProzent = 7,
        [Display(Name = "19 %")]
        NeunzehnProzent = 19
    }
}
