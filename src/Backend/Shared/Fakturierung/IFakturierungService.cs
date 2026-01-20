using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Shared.Fakturierung
{
    public interface IFakturierungService
    {
        Rechnung ErstelleRechnung(List<Rechnungsposten> posten, Firma firma, Kunde kunde, string userInitialen);
        List<Rechnungsposten> GeneriereRechnungsposten(List<(Leistung leistung, TimeSpan dauer)> leistungenMitDauer, decimal mengeZusatzLogik = 0);
        void RechnungAbschliessen(Rechnung rechnung);
    }
}