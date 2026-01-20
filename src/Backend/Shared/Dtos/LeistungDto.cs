using Shared.Domain.Enums;

namespace Shared.Dtos
{
    public class LeistungDto
    {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string Bezeichnung { get; set; }
        public string? Beschreibung { get; set; }
        public TimeSpan Richtzeit { get; set; }
        public decimal Pauschalpreis { get; set; }
        public TimeSpan Pauschalgrenze { get; set; }
        public decimal PreisPro15Min { get; set; }
        public bool IstVorOrt { get; set; }
        public bool HatZusatzlogik { get; set; }
        public ZusatzlogikDto? Zusatzlogik { set; get; }
        public Steuersatz Steuersatz { get; set; }
    }
}
