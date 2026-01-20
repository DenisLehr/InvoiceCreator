using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Dtos.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EmailMusterTyp
    {
        [Display(Name = "Terminbestätigung")]
        TerminBestaetigungKunde,
        [Display(Name = "Terminbestätigung Kundentermin")]
        TerminBestaetigungServicetechniker,
        [Display(Name = "Rechnungsversand")]
        RechnungVersendet,
        [Display(Name = "Registrierung abgeschlossen")]
        RegistrierungWillkommen,
        [Display(Name = "Passwort zurücksetzen")]
        PasswortZuruecksetzen
    }
}
