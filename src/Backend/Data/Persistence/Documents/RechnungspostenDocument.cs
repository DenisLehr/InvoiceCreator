namespace Data.Persistence.Documents
{
    public class RechnungspostenDocument
    {
        public string LeistungID { get; set; }
        public string Bezeichnung { get; set; }
        public string? Beschreibung { get; set; }
        public decimal Menge {  get; set; }
        public decimal Einzelpreis { get; set; }
        public decimal Steuersatz {  get; set; }
        public decimal Rabatt {  get; set; }
        public string Einheit { get; set; }
        public int Position { get; set; }
        public decimal GesamtNettopreis { get; set; }
        public decimal GesamtBruttopreis { get; set; }
        public decimal Steuerbetrag { get; set; }

    }
}
