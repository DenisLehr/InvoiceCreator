using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;

namespace Data.Interfaces
{
    public interface IKundeRepository : IBaseRepository<Kunde>
    {
        Task<Adresse> GetAdresseVonKundeAsync(string id);
        Task<List<KundeDto>> GetKundenNamenAsync(string teileingabe);
        Task <PaginiertesResultDto<KundeDto>> GetPaginierteKunden(int seite, int eintraegeProSeite, string? teileingabe);
        
    }
}
