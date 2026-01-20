using Shared.Domain.Models;
using Shared.Dtos;

namespace Data.Interfaces
{
    public interface IRechnungRepository : IBaseRepository<Rechnung>
    {
        Task<List<Rechnung>> GetRechnungenVonKundeAsync(string kundeId);

        Task<Rechnung> GetRechnungMitBestellreferenzAsync(string kundenReferenz);

        Task<Rechnung> GetRechnungMitRechnungsNummerAsync(string rechnungsNummer);
        Task<List<Rechnung>> GetOffeneRechnungenAsync();
        Task<List<Rechnung>> GetRechnungenImZeitraum(DateTime startDatum, DateTime endDatum);
        Task<PaginiertesResultDto<RechnungDto>> GetPaginierteRechnungen(int seite, int eintraegeProSeite, string? teileingabe);
    }
}
