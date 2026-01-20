namespace Data.Persistence.Documents
{
    public class KundeDocument : BaseDocument
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string? Firmenname { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string? TelefonMobil { get; set; }
        public DateTime Geburtsdatum { get; set; }
        public string Geschlecht { get; set; }
        public AdresseDocument Adresse { get; set; }
        public List<TerminDocument>? Termine { get; set; }
        public decimal KundenRabatt { get; set; }
        public bool IstB2B { get; set; }
    }
}
