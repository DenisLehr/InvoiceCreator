using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Geschlecht
    {
        [Display(Name = "männlich")]
        maennlich,
        weiblich,
        divers
    }
}
