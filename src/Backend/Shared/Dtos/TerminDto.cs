using Shared.Domain.Enums;

namespace Shared.Dtos
{
    public class TerminDto
    {
        public string? Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Text { get; set; }
        public string KundeId { get; set; }
        public string? UserId { get; set; }
        public List<string> Leistungen { get; set; }
        public TimeSpan GeschätzteDauer { get; set; }
        public Terminstatus Status { get; set; }
        public DateTime? BestätigtAm { get; set; }
    }
}
