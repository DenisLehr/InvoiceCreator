using Shared.Domain.Enums;

namespace Shared.Dtos
{
    public class RechnungspostenDto
    {
        public string LeistungID { get; set; }
        public string Bezeichnung { get; set; }
        public string? Beschreibung { get; set; }
        public decimal Menge { get; set; }
        public decimal Einzelpreis { get; set; }
        public Steuersatz Steuersatz { get; set; }
        public decimal Rabatt { get; set; }
        public Einheit Einheit { get; set; }
        public int Position { get; set; }
        public decimal GesamtNettopreis { get; set; }
        public decimal GesamtBruttopreis { get; set; }
        public decimal Steuerbetrag { get; set; }
    }
}
