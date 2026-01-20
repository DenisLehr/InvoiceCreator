using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Models
{
    public class Leistung: BaseModel
    {
        public Leistung()
        {
            Zusatzlogik = new();
        }
        public string? Code { get; set; }

        [Required(ErrorMessage = "Die Bezeichnung ist erforderlich.")]
        [StringLength(100, ErrorMessage = "Die Bezeichnung darf maximal 100 Zeichen lang sein.")]
        [RegularExpression(@"^[a-zA-Z0-9äöüÄÖÜß\s,\.\-\/\(\):\+→]+$", ErrorMessage = "Die Bezeichnung enthält ungültige Zeichen.")]
        public string Bezeichnung { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Die Beschreibung darf maximal 500 Zeichen lang sein.")]
        [RegularExpression(@"^[a-zA-Z0-9äöüÄÖÜß\s,\.\-\/\(\):\+→]*$", ErrorMessage = "Die Beschreibung enthält ungültige Zeichen.")]
        public string? Beschreibung { get; set; }

        [Required(ErrorMessage = "Die Richtzeit ist erforderlich.")]
        public TimeSpan Richtzeit { get; set; }
        [Required(ErrorMessage = "Der Pauschalpreis ist erforderlich.")]
        [Range(0, double.MaxValue, ErrorMessage = "Der Pauschalpreis muss positiv sein.")]
        public decimal Pauschalpreis { get; set; }
        [Required(ErrorMessage = "Die Pauschalgrenze ist erforderlich.")]
        public TimeSpan Pauschalgrenze { get; set; }
        [Required(ErrorMessage = "Der Preis pro 15 Min nach Ablauf der Pauschalzeit ist erforderlich.")]
        [Range(0, double.MaxValue, ErrorMessage = "Der Preis pro 15 Minuten muss positiv sein.")]
        public decimal PreisPro15Min { get; set; }

        public bool IstVorOrt { get; set; }

        public bool HatZusatzlogik { get; set; }

        public Zusatzlogik? Zusatzlogik { get; set; }

        [Required(ErrorMessage = "Der Steuersatz ist erforderlich.")]
        [EnumDataType(typeof(Steuersatz))]
        public Steuersatz Steuersatz { get; set; } = Steuersatz.NeunzehnProzent;

    }
}
