using Shared.Domain.Models;
using Shared.Dtos;

namespace Data.Interfaces
{
    public interface ILeistungRepository : IBaseRepository<Leistung>
    {
        Task<string?> GetLetzterLeistungCodeAsync();
        Task<PaginiertesResultDto<LeistungDto>> GetPaginierteLeistungen(int seite, int eintraegeProSeite, string? teileingabe);
    }
}
