using Shared.Contracts.Responses;
using Shared.Dtos;

namespace Services.Interfaces
{
    public interface IFirmendatenService
    {
        Task<BaseResponse<bool>> CreateFirma(FirmaDto dto);
        Task<BaseResponse<bool>> DeleteFirma(string id);
        Task<FirmaDto> GetFirmendatenAsync();
        Task<BaseResponse<bool>> UpdateFirma(FirmaDto dto);
    }
}