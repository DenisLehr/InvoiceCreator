using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.ValueObjects
{
    public class Bankverbindung
    {
        [Required(ErrorMessage = "Der Kontoinhaber ist erforderlich.")]
        [StringLength(80, ErrorMessage = "Der Kontoinhaber darf maximal 80 Zeichen lang sein.")]
        public string Kontoinhaber { get; set; }

        [Required(ErrorMessage = "Die IBAN ist erforderlich.")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{11,30}$", ErrorMessage = "Ungültige IBAN.")]
        public string IBAN { get; set; }

        [Required(ErrorMessage = "Die BIC ist erforderlich.")]
        [RegularExpression(@"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$", ErrorMessage = "Ungültige BIC.")]
        public string BIC { get; set; }

        [Required(ErrorMessage = "Der Bankname ist erforderlich.")]
        [StringLength(100, ErrorMessage = "Der Bankname darf maximal 100 Zeichen lang sein.")]
        public string Bankname { get; set; }

        public bool Aktiv { get; set; } = true;
    }
}
