using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;

namespace Shared.Domain.Models  
{
    public class Rechnung: BaseModel
    {
        public string Rechnungsnummer { get; set; }
        public string? Bestellreferenz {  get; set; }
        public DateTime Rechnungsdatum { get; set; }
        public string KundeID { get; set; }
        public DateTime LeistungszeitraumVon {  get; set; } = DateTime.UtcNow;
        public DateTime LeistungszeitraumBis { get; set; } = DateTime.UtcNow;
        public DateTime Faelligkeit { get; set; } = DateTime.UtcNow.AddDays(14);
        public Waehrung Waehrung { get; set; } = Waehrung.EUR;
        public string Zahlungsbedingungen { get; set; }
        public List<Rechnungsposten> Rechnungsposten { get; set; }
       
        // absoluter Wert von 0 - 100
        public decimal Skonto { get; set; } = 0;
        // absoluter Wert von 0 - 100
        public decimal Rabatt { get; set; } = 0;
        public Zahlungsstatus Zahlungsstatus { get; set; } = Zahlungsstatus.Offen;
        public bool HatBestellReferenz { get; set; } = false;
        public DateTime VersendetAm {  get; set; }
        public string ErstelltVon {  get; set; }
        public byte[]? Unterschrift {  get; set; }

        // berechnete Felder
        public decimal PostenNettoSumme => Math.Round(Rechnungsposten.Sum(p => p.GesamtNettopreis), 2);
        public decimal PostenBruttoSumme => Math.Round(Rechnungsposten.Sum(p => p.GesamtBruttopreis), 2);
        public decimal PostenSteuerSumme => Math.Round(Rechnungsposten.Sum(p => p.Steuerbetrag), 2);
        public decimal Rabattbetrag => Math.Round(PostenNettoSumme * Rabatt / 100, 2);
        public decimal BruttobetragNachRabatt => Math.Round(PostenBruttoSumme - Rabattbetrag, 2);
        public decimal Skontobetrag => Math.Round(BruttobetragNachRabatt * Skonto / 100, 2);
        public decimal BruttoRechnungsBetrag => BruttobetragNachRabatt - Skontobetrag;
        
    }
}
