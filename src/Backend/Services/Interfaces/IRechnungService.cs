using Shared.Contracts.Responses;
using Shared.Dtos;

namespace Services.Interfaces
{
    public interface IRechnungService
    {
        Task<PaginiertesResultDto<RechnungDto>> GetPagedRechnungenAsync(string? teileingabe, int seite, int eintraegeProSeite);
        Task<BaseResponse<RechnungDto>> GetRechnungByIdAsync(string id);
        Task<BaseResponse<RechnungDto>> GetRechnungByRechnungsnummerAsync(string rechnungsnummer);
        Task<BaseResponse<List<RechnungDto>>> GetRechnungenAsync();
        Task<BaseResponse<bool>> SpeicherRechnungAsync(RechnungDto value);
    }
}