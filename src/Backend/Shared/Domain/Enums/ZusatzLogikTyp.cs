using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ZusatzLogikTyp
    {
        [Display(Name = "GB Zuschlag")]
        GB_Zuschlag,
        [Display(Name = "Benutzer Staffelung")]
        Benutzerstaffel,
        [Display(Name = "Lizenzen Staffelung")]
        Lizenzstaffel, 
        Standard
    }
}
