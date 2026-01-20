using Shared.Domain.Models;
using Shared.Dtos;

namespace Data.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsUsernameTakenAsync(string username);
        Task<PaginiertesResultDto<UserDto>> GetPaginierteUser(int seite, int eintraegeProSeite, string? teileingabe);

    }
}
