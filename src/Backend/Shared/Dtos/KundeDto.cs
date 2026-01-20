using Shared.Domain.Enums;

namespace Shared.Dtos
{
    public class KundeDto
    {
        public string? Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string? Firmenname { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string? TelefonMobil { get; set; }
        public DateTime Geburtsdatum { get; set; }
        public Geschlecht Geschlecht { get; set; }
        public AdresseDto Adresse { get; set; }
        public TerminDto? NaechsterTermin {  get; set; }
        public List<TerminDto>? Termine { get; set; }
        public decimal KundenRabatt { get; set; }
        public DateTime GeaendertAm { get; set; }
        public bool IstB2B { get; set; }
    }
}
