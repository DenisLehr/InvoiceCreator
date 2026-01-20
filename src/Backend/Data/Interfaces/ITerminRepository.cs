using Shared.Domain.Models;
using Shared.Dtos;

namespace Data.Interfaces
{
    public interface ITerminRepository: IBaseRepository<Termin>
    {
        Task<PaginiertesResultDto<TerminDto>> GetPaginierteTermine(int seite, int eintraegeProSeite, string? teileingabe);
    }
}