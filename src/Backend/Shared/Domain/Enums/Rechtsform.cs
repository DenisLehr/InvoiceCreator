using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Rechtsform
    {
        [Display(Name = "Einzelunternehmen")]
        Einzelunternehmen = 1,

        [Display(Name = "eingetragener Kaufmann")]
        EK = 2,                    

        [Display(Name = "Gesellschaft bürgerlichen Rechts")]
        GbR = 3,

        [Display(Name = "Offene Handelsgesellschaft")]
        OHG = 4,

        [Display(Name = "Kommanditgesellschaft")]
        KG = 5,

        [Display(Name = "GmbH")]
        GmbH = 6,

        [Display(Name = "UG (haftungsbeschränkt)")]
        UG = 7,

        [Display(Name = "GmbH & Co. KG")]
        GmbHCoKG = 8,

        [Display(Name = "Aktiengesellschaft")]
        AG = 9,

        [Display(Name = "eingetragene Genossenschaft")]
        eG = 10,

        [Display(Name = "Verein (e.V.)")]
        Verein = 11,

        [Display(Name = "Sonstige / unbekannt")]
        Sonstige = 99
    }
}
