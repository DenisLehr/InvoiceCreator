using Shared.Contracts.Responses;
using Shared.Dtos;
using Shared.Dtos.Email;

namespace Services.Interfaces
{
    public interface ITerminService
    {
        Task<BaseResponse<bool>> CreateTerminAsync(TerminDto dto);
        Task<BaseResponse<bool>> DeleteTerminAsync(string id);
        Task<PaginiertesResultDto<TerminDto>> GetPagedTermineAsync(string? teileingabe, int seite, int eintraegeProSeite);
        Task<BaseResponse<TerminDto>> GetTerminByIdAsync(string id);
        Task<BaseResponse<List<TerminDto>>> GetTermineAsync();
        Task<BaseResponse<bool>> SendeTerminAnKundeAsync(TerminDto termin);
        Task<BaseResponse<bool>> SendeTerminAnServicetechnikerAsync(TerminDto termin);
        Task<BaseResponse<bool>> UpdateTerminAsync(TerminDto dto);
    }
}