namespace Data.Persistence.Documents
{
    public class FirmaDocument : BaseDocument
    {
        public string Name { get; set; }
        public string Kontaktperson { get; set; }
        public List<string>? Geschaeftsfuehrer {  get; set; }
        public AdresseDocument Adresse { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string UStId { get; set; }
        public BankverbindungDocument Bankverbindung { get; set; }
        public string? HandelsregisterNr { get; set; }
        public string? Registergericht { get; set; }
        public string? Rechtsform { get; set; }
    }
}
