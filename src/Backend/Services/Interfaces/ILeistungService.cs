using Shared.Contracts.Responses;
using Shared.Dtos;

namespace Services.Interfaces
{
    public interface ILeistungService
    {
        Task<BaseResponse<bool>> CreateLeistungAsync(LeistungDto dto);
        Task<BaseResponse<bool>> DeleteLeistungAsync(string id);
        Task<BaseResponse<LeistungDto>> GetLeistungByIdAsync(string id);
        Task<BaseResponse<List<LeistungDto>>> GetLeistungenAsync();
        Task<PaginiertesResultDto<LeistungDto>> GetPagedLeistungenAsync(string? teileingabe, int seite, int eintraegeProSeite);
        Task<BaseResponse<bool>> UpdateLeistungAsync(LeistungDto dto);
    }
}