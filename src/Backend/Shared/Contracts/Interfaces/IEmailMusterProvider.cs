using Shared.Dtos.Enums;

namespace Shared.Contracts.Interfaces
{
    public interface IEmailMusterProvider
    {
        string GetMuster(EmailMusterTyp typ, Dictionary<string, string> platzhalter);
    }
}
