using Shared.Domain.Models;

namespace Shared.PdfGenerator
{
    public interface IPdfGeneratorService
    {
        byte[] GeneriereRechnung(Rechnung rechnung, Kunde kunde, Firma firma, byte[]? logo = null);
    }
}
