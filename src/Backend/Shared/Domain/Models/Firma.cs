using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Models
{
    public class Firma: BaseModel
    {
        [Required(ErrorMessage = "Der Firmenname ist erforderlich.")]
        [StringLength(100, ErrorMessage = "Der Firmenname darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Die Kontaktperson ist erforderlich.")]
        [StringLength(80, ErrorMessage = "Die Kontaktperson darf maximal 80 Zeichen lang sein.")]
        public string Kontaktperson { get; set; }
        [Display(Name = "Geschäftsführer")]
        public List<string>? Geschaeftsfuehrer { get; set; }

        [Required(ErrorMessage = "Die Adresse ist erforderlich.")]
        public Adresse Adresse { get; set; } = new();

        [Required(ErrorMessage = "Die E-Mail-Adresse ist erforderlich.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Die Telefonnummer ist erforderlich.")]
        [Phone(ErrorMessage = "Ungültige Telefonnummer.")]
        public string Telefon { get; set; }

        [RegularExpression(@"^DE[0-9]{9}$", ErrorMessage = "Die USt-Id muss im Format DE123456789 sein.")]
        public string? UStId { get; set; }

        [Required(ErrorMessage = "Die Bankverbindung ist erforderlich.")]
        public Bankverbindung Bankverbindung { get; set; } = new();
        public byte[]? Logo { get; set; }
        [Display(Name = "Handelsregister‑Nummer")]
        [RegularExpression(@"^(HRB|HRA)\s?\d{1,8}$", ErrorMessage = "Handelsregister‑Nummer muss im Format HRB 12345 oder HRA 12345 sein.")]
        public string? HandelsregisterNr { get; set; }

        [StringLength(100, ErrorMessage = "Registergericht darf max. 100 Zeichen haben.")]
        public string? Registergericht { get; set; }
        public Rechtsform? Rechtsform { get; set; }
    }
}
