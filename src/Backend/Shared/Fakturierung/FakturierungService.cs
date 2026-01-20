using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Fakturierung;

namespace Services.Fakturierung
{
    public class FakturierungService : IFakturierungService
    {
        public List<Rechnungsposten> GeneriereRechnungsposten(List<(Leistung leistung, TimeSpan dauer)> leistungenMitDauer, decimal mengeZusatzLogik = 0)
        {
            var position = 0;
            var rechnungsPosten = new List<Rechnungsposten>();

            foreach (var (leistung, dauer) in leistungenMitDauer)
            {
                position++;

                var nettoPreis = BerechneRechnungspostenPreis(leistung, dauer, mengeZusatzLogik);

                var posten = new Rechnungsposten
                {
                    LeistungID = leistung.Id,
                    Bezeichnung = leistung.Bezeichnung,
                    Beschreibung = leistung.Beschreibung,
                    Steuersatz = leistung.Steuersatz,
                    Einzelpreis = nettoPreis,
                    Position = position
                };
                rechnungsPosten.Add(posten);
            }
            return rechnungsPosten;
        }

        private decimal BerechneRechnungspostenPreis(Leistung leistung, TimeSpan dauer, decimal mengeZusatzLogik)
        {
            var zusatzlogikZuschlag = leistung.HatZusatzlogik? BerechneZusatzlogikPreis(leistung, mengeZusatzLogik) : 0m;

            if (dauer <= leistung.Pauschalgrenze) 
                return Math.Round(leistung.Pauschalpreis + zusatzlogikZuschlag, 2);

            var restzeit = dauer - leistung.Pauschalgrenze;
            var abschnitte = Math.Ceiling(restzeit.TotalMinutes / 15);
            var zuschlag = (decimal)abschnitte * leistung.PreisPro15Min;

            var gesamt = leistung.Pauschalpreis + zuschlag;

            return Math.Round(gesamt + zusatzlogikZuschlag, 2);
        }

        private decimal BerechneZusatzlogikPreis(Leistung leistung, decimal menge)
        {
                var grenze = leistung.Zusatzlogik.Grenze;
                var preisProEinheit = leistung.Zusatzlogik.PreisProEinheit;
                
                var einheiten = Math.Ceiling(menge / grenze);

                return Math.Round(einheiten * preisProEinheit, 2);
        }

        public Rechnung ErstelleRechnung(List<Rechnungsposten> posten, Firma firma, Kunde kunde, string userInitialen)
        {
            var erstellDatum = DateTime.UtcNow;
            var rechnungsPosten = posten;

            var rechnung = new Rechnung
            {
                Rechnungsdatum = erstellDatum,
                Rechnungsnummer = GeneriereRechnungsNr(erstellDatum, userInitialen),
                KundeID = kunde.Id,
                Rechnungsposten = rechnungsPosten,
                Zahlungsbedingungen = "Bitte überweisen Sie den Rechnungsbetrag innerhalb von 14 Tagen ohne Abzug.",
                Zahlungsstatus = Zahlungsstatus.Offen
            };
            return rechnung;
        }

        private string GeneriereRechnungsNr(DateTime datum, string userInitialen)
        {
            if (string.IsNullOrWhiteSpace(userInitialen))
                userInitialen = "CS";

            var rechnungsNr = string.Join("-", "RE", datum.ToString("yyyyMMddHHmmss"), Random.Shared.Next(10,100), userInitialen);
            return rechnungsNr;
        }

        public void RechnungAbschliessen(Rechnung rechnung)
        {
            if (rechnung.Zahlungsstatus != Zahlungsstatus.Offen)
                throw new InvalidOperationException("Nur offene Rechnungen können abgeschlossen werden.");

            rechnung.Zahlungsstatus = Zahlungsstatus.Bezahlt;
            rechnung.GeaendertAm = DateTime.UtcNow;
        }
    }
}
