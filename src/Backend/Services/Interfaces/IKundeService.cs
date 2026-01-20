using Shared.Contracts.Responses;
using Shared.Dtos;
using Shared.Dtos.Email;

namespace Services.Interfaces
{
    public interface IKundeService
    {
        Task<BaseResponse<bool>> CreateKundeAsync(KundeDto dto);
        Task<BaseResponse<bool>> DeleteKundeAsync(string id);
        Task<BaseResponse<KundeDto>> GetKundeByIdAsync(string id);
        Task<BaseResponse<List<KundeDto>>> GetKundenAsync();
        Task<BaseResponse<List<KundeDto>>> GetKundenByNameAsync(string name);
        Task<BaseResponse<bool>> SendeEmailAnKundeAsync(SendeEmailRequestDto emailRequestDto);
        Task<PaginiertesResultDto<KundeDto>> GetPagedKundenAsync(string? teileingabe, int seite, int eintraegeProSeite);
        Task<BaseResponse<bool>> UpdateKundeAsync(KundeDto dto);
    }
}