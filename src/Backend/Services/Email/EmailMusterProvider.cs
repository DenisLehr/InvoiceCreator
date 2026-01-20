using Shared.Contracts.Interfaces;
using Shared.Dtos.Enums;

namespace Application.Common.Services
{
    public class EmailMusterProvider:IEmailMusterProvider
    {
        private readonly Dictionary<EmailMusterTyp, string> _muster = new()
        {
            [EmailMusterTyp.TerminBestaetigungKunde] = "Sehr geehrte/r {{Name}},\n\nIhr Servicetermin ist am {{Datum}} um {{Uhrzeit}} Uhr.\n\n" +
            "Mit freundlichen Grüßen\n{{Ansprechpartner}}\n{{Firmenname}}",
            [EmailMusterTyp.TerminBestaetigungServicetechniker] = "Service-Termin am {{Datum}} um {{Uhrzeit}} Uhr bei {{Name}}.\n\n" +
            "Adresse:\n{{Strasse}} {{Hausnummer}} {{Hausnummerzusatz}}\n{{PLZ}} {{Stadt}}\n\n" +
            "Serviceleistungen:\n{{Leistungen}}\n\nGeschätzte Dauer:\n{{Dauer}}\n\nNotizen:\n{{Notiz}}",
            [EmailMusterTyp.RechnungVersendet] = "Ihre Rechnung Nr. {{Rechnungsnummer}} wurde am {{Datum}} versendet.",
            [EmailMusterTyp.RegistrierungWillkommen] = "Willkommen, {{Name}}! Ihr Konto wurde erfolgreich erstellt.",
            [EmailMusterTyp.PasswortZuruecksetzen] = "Klicken Sie hier, um Ihr Passwort zurückzusetzen: {{Link}}"
        };

        public string GetMuster(EmailMusterTyp typ, Dictionary<string, string> platzhalter)
        {
            if (!_muster.TryGetValue(typ, out var muster))
                throw new ArgumentException($"Kein Template für {typ} gefunden.");

            foreach (var kv in platzhalter)
            {
                muster = muster.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            return muster;
        }
    }
}
