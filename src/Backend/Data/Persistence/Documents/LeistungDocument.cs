namespace Data.Persistence.Documents
{
    public class LeistungDocument : BaseDocument
    {
        public string? Code { get; set; }
        public string Bezeichnung { get; set; }
        public string? Beschreibung { get; set; }
        public TimeSpan Richtzeit { get; set; }
        public decimal Pauschalpreis { get; set; }
        public TimeSpan Pauschalgrenze { get; set; }
        public decimal PreisPro15Min { get; set; }
        public bool IstVorOrt { get; set; }
        public bool HatZusatzlogik { get; set; }
        public ZusatzlogikDocument? Zusatzlogik { set; get; }
        public string Steuersatz {  get; set; }
    }
}
