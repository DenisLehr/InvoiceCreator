using Shared.Domain.Enums;

namespace Shared.Dtos
{
    public class FirmaDto
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Kontaktperson { get; set; }
        public List<string>? Geschaeftsfuehrer { get; set; }
        public AdresseDto Adresse { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string UStId { get; set; }
        public BankverbindungDto Bankverbindung { get; set; }
        public byte[]? Logo { get; set; }
        public string? HandelsregisterNr { get; set; }
        public string? Registergericht { get; set; }
        public Rechtsform? Rechtsform { get; set; }
    }
}
