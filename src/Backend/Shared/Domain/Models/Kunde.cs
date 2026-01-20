using Shared.Domain.Enums;
using Shared.Domain.Validation;
using Shared.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Models
{
    public class Kunde: BaseModel
    {
        [Required(ErrorMessage = "Vorname ist erforderlich.")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZäöüÄÖÜß\s\-]+$", ErrorMessage = "Nur Buchstaben, Leerzeichen und Bindestriche erlaubt.")]
        public string Vorname { get; set; }
        [Required(ErrorMessage = "Nachname ist erforderlich.")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZäöüÄÖÜß\s\-]+$", ErrorMessage = "Nur Buchstaben, Leerzeichen und Bindestriche erlaubt.")]
        public string Nachname { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9äöüÄÖÜß&.,\-()'""\s]{2,100}$",
            ErrorMessage = "Der Firmenname darf nur Buchstaben, Zahlen und bestimmte Sonderzeichen enthalten.")]
        [StringLength(100,
            ErrorMessage = "Der Firmenname muss zwischen 2 und 100 Zeichen lang sein.")]
        public string? Firmenname { get; set; }
        [Required(ErrorMessage = "E-Mail ist erforderlich.")]
        [EmailAddress(ErrorMessage = "Bitte eine gültige E-Mail-Adresse eingeben.")]
        [StringLength(100, ErrorMessage = "Die E-Mail-Adresse darf maximal 100 Zeichen lang sein.")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Telefon ist erforderlich.")]
        [Phone]
        public string Telefon { get; set; }

        [Phone]
        public string? TelefonMobil { get; set; }
        [Required(ErrorMessage = "Geburtsdatum ist erforderlich.")]
        [DataType(DataType.Date)]
        [DatumNichtInZukunft(ErrorMessage = "Geburtsdatum darf nicht in der Zukunft liegen.")]
        public DateTime Geburtsdatum { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "Geschlecht ist erforderlich.")]
        [EnumDataType(typeof(Geschlecht))]
        public Geschlecht Geschlecht { get; set; }
        public Adresse Adresse { get; set; } = new();

        public Termin? NaechsterTermin { get; set; } = null;
        public List<Termin> Termine { get; set; } = new();
        [Range(0, 100)]
        [Display(Name = "Kundenrabatt")]
        public decimal KundenRabatt { get; set; } = 0;

        public bool IstB2B => !string.IsNullOrWhiteSpace(Firmenname);

    }
}
