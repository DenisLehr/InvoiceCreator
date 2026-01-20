using Shared.Contracts.Responses;
using Shared.Dtos.Email;

namespace Shared.Contracts.Interfaces
{
    public interface IEmailService
    {
        public Task<BaseResponse<bool>> SendeNachrichtAnBuchhaltungAsync(EmailDto email);
        public Task<BaseResponse<bool>> SendeNachrichtAsync(EmailDto email);
        
    }
}
