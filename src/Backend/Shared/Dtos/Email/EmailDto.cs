namespace Shared.Dtos.Email
{
    public class EmailDto
    {
        public string EmpfaengerEmail { get; set; } = string.Empty;
        public string Betreff { get; set; } = string.Empty;
        public string Nachricht { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = false;
        public List<EmailAnhangDto>? Anhang { get; set; }
    }
}
