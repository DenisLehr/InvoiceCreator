using Shared.Dtos.Enums;

namespace Shared.Dtos.Email
{
    public class SendeEmailRequestDto
    {
        public string KundeId { get; set; }
        public EmailDto Email { get; set; }
        public EmailMusterTyp Muster {  get; set; }
    }
}
