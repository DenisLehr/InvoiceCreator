using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Waehrung
    {
        [Display(Name = "€")]
        EUR
    }
}
