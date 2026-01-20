namespace Data.Persistence.Documents
{
    public class TerminDocument:BaseDocument
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Text { get; set; } = string.Empty;
        public string KundeId { get; set; }
        public string? UserId { get; set; }
        public List<string> Leistungen { get; set; }
        public TimeSpan GeschätzteDauer { get; set; }
        public string Status { get; set; }
        public DateTime? BestätigtAm { get; set; }
    }
}
