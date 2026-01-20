namespace Data.Persistence.Documents
{
    public class RechnungDocument : BaseDocument
    {
        public string Rechnungsnummer { get; set; }
        public string? Bestellreferenz { get; set; }
        public DateTime Rechnungsdatum { get; set; }
        public string KundeID { get; set; }
        public DateTime LeistungszeitraumVon { get; set; }
        public DateTime LeistungszeitraumBis { get; set; }
        public DateTime Faelligkeit { get; set; }
        public string Waehrung { get; set; }
        public string Zahlungsbedingungen { get; set; }
        public List<RechnungspostenDocument> Rechnungsposten { get; set; }
        public decimal Skonto { get; set; }
        public decimal Rabatt { get; set; }
        public string Zahlungsstatus { get; set; }
        public bool HatBestellReferenz { get; set; }
        public DateTime VersendetAm { get; set; }
        public string ErstelltVon { get; set; }
        public byte[]? Unterschrift { get; set; }
    }
}
