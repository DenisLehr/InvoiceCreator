using Shared.Contracts.Responses;
using Shared.Dtos;

namespace Services.Interfaces
{
    public interface IRechnungsVerarbeitungsService
    {
        Task<BaseResponse<RechnungPdfResponseDto>> CreateRechnungAsync(CreateRechnungDto dto);
    }
}