using Shared.Domain.Enums;

namespace Shared.Dtos
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Initialen { get; set; }
        public string Email { get; set; }
        public Rolle Rolle { get; set; }
        public DateTime? LetzterLogin { get; set; }
    }
}
