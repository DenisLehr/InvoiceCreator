using Shared.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.ValueObjects
{
    public class Adresse
    {
        [Required(ErrorMessage = "Straße ist erforderlich.")]
        [StringLength(50, ErrorMessage = "Straße darf maximal 50 Zeichen lang sein.")]
        public string Strasse { get; set; }

        [Required(ErrorMessage = "Hausnummer ist erforderlich.")]
        [RegularExpression(@"^\d{1,4}$", ErrorMessage = "Hausnummer muss eine Zahl zwischen 1 und 9999 sein.")]
        public string Hausnummer { get; set; }

        [StringLength(1, ErrorMessage = "Hausnummerzusatz darf maximal 1 Zeichen enthalten.")]
        public string? Hausnummerzusatz { get; set; }

        [Required(ErrorMessage = "Stadt ist erforderlich.")]
        [StringLength(50, ErrorMessage = "Stadt darf maximal 50 Zeichen lang sein.")]
        public string Stadt { get; set; }

        [Required(ErrorMessage = "PLZ ist erforderlich.")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "PLZ muss genau 5 Ziffern enthalten.")]
        public string PLZ { get; set; }

        [Required(ErrorMessage = "Land ist erforderlich.")]
        [EnumDataType(typeof(Land))]
        public Land Land { get; set; }
    }
}
