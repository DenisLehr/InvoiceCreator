using Shared.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Models
{
    public class Termin: BaseModel
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        public string KundeId { get; set; }
        public string? UserId { get; set; }

        public List<string> Leistungen { get; set; } = new();

        public TimeSpan GeschätzteDauer { get; set; }

        public Terminstatus Status { get; set; } = Terminstatus.Geplant;

        public DateTime? BestätigtAm { get; set; }
    }
}
