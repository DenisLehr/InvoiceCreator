using Shared.Contracts.Responses;
using Shared.Dtos;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<bool>> CreateUserAsync(UserDto dto);
        Task<BaseResponse<bool>> DeleteUserAsync(string id);
        Task<PaginiertesResultDto<UserDto>> GetPagedUsersAsync(string? teileingabe, int seite, int eintraegeProSeite);
        Task<BaseResponse<UserDto>> GetUserByIdAsync(string id);
        Task<BaseResponse<List<UserDto>>> GetUsersAsync();
        Task<BaseResponse<bool>> UpdateUserAsync(UserDto dto);
    }
}