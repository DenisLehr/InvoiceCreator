using Shared.Domain.Enums;

namespace Shared.Dtos
{
    public class RechnungDto
    {
        public string? Id { get; set; }
        public string Rechnungsnummer { get; set; }
        public string? Bestellreferenz { get; set; }
        public DateTime Rechnungsdatum { get; set; }
        public string KundeID { get; set; }
        public DateTime LeistungszeitraumVon { get; set; }
        public DateTime LeistungszeitraumBis { get; set; }
        public DateTime Faelligkeit { get; set; }
        public Waehrung Waehrung { get; set; }
        public string Zahlungsbedingungen { get; set; }
        public List<RechnungspostenDto> Rechnungsposten { get; set; }
        public decimal Skonto { get; set; }
        public decimal Rabatt { get; set; }
        public Zahlungsstatus Zahlungsstatus { get; set; }
        public DateTime VersendetAm { get; set; }
        public string ErstelltVon { get; set; }
        public byte[]? Unterschrift { get; set; }
    }
}
