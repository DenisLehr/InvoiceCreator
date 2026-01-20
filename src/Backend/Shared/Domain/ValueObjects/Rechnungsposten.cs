using Shared.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.ValueObjects
{
    public class Rechnungsposten
    {
        [Required]
        public string LeistungID { get; set; }
        [Required(ErrorMessage = "Die Bezeichnung ist erforderlich.")]
        [StringLength(100, ErrorMessage = "Die Bezeichnung darf maximal 100 Zeichen lang sein.")]
        [RegularExpression(@"^[a-zA-Z0-9äöüÄÖÜß\s,\.\-\/\(\):\+→]+$", ErrorMessage = "Die Bezeichnung enthält ungültige Zeichen.")]
        public string Bezeichnung { get; set; }
        [StringLength(500, ErrorMessage = "Die Beschreibung darf maximal 500 Zeichen lang sein.")]
        [RegularExpression(@"^[a-zA-Z0-9äöüÄÖÜß\s,\.\-\/\(\):\+→]*$", ErrorMessage = "Die Beschreibung enthält ungültige Zeichen.")]
        public string? Beschreibung { get; set; }
        [Required(ErrorMessage = "Die Menge ist erforderlich.")]
        [Range(0, 1000, ErrorMessage = "Die Menge muss positiv sein.")]
        public decimal Menge {  get; set; }
        [Required(ErrorMessage = "Der Einzelpreis ist erforderlich.")]
        [Range(0, 1000, ErrorMessage = "Der Einzelpreis muss positiv sein.")]
        public decimal Einzelpreis { get; set; }
        [Required(ErrorMessage = "Der Steuersatz ist erforderlich.")]
        [EnumDataType(typeof(Steuersatz))]
        public Steuersatz Steuersatz { get; set; } = Steuersatz.NeunzehnProzent;
        [Range(0, 100, ErrorMessage = "Der Rabatt muss zwischen 0 und 100 liegen.")]
        public decimal Rabatt { get; set; } = 0;
        [EnumDataType(typeof(Einheit))]
        public Einheit Einheit { get; set; }
        public int Position { get; set; }

        // berechnete Felder
        public decimal GesamtNettopreis => Math.Round(Menge * Einzelpreis - (Menge * Einzelpreis * Rabatt / 100), 2);
        public decimal GesamtBruttopreis => Math.Round(GesamtNettopreis + (GesamtNettopreis * (decimal)Steuersatz / 100), 2);
        public decimal Steuerbetrag => Math.Round(GesamtNettopreis * ((decimal)Steuersatz / 100), 2);

    }
}
