using Shared.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Models
{
    public class User: BaseModel
    {
        [Required(ErrorMessage = "Der Benutzername ist erforderlich.")]
        [StringLength(50, ErrorMessage = "Der Benutzername darf maximal 50 Zeichen lang sein.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vorname ist erforderlich.")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZäöüÄÖÜß\s\-]+$", ErrorMessage = "Nur Buchstaben, Leerzeichen und Bindestriche erlaubt.")]
        public string Vorname { get; set; }
        [Required(ErrorMessage = "Nachname ist erforderlich.")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZäöüÄÖÜß\s\-]+$", ErrorMessage = "Nur Buchstaben, Leerzeichen und Bindestriche erlaubt.")]
        public string Nachname { get; set; }

        public string Initialen => $"{char.ToUpper(Vorname[0])}{char.ToUpper(Nachname[0])}";

        [Required(ErrorMessage = "E-Mail ist erforderlich.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Rolle ist erforderlich.")]
        [EnumDataType(typeof(Rolle))]
        public Rolle Rolle { get; set; }

        [Display(Name = "Letzter Login")]
        public DateTime? LetzterLogin { get; set; }
    }
}
