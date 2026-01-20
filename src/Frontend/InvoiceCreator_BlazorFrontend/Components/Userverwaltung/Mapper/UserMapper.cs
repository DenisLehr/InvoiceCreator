using Shared.Domain.Models;
using Shared.Domain.Enums;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Userverwaltung.Mapper
{
    public static class UserMapper
    {
        public static UserDto ToCreateDto(User user) => new()
        {
            UserName = user.UserName,
            Vorname = user.Vorname,
            Nachname = user.Nachname,
            Email = user.Email,
            Rolle = user.Rolle,
            Initialen = user.Initialen
        };

        public static UserDto ToUpdateDto(User user) => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Vorname = user.Vorname,
            Nachname = user.Nachname,
            Email = user.Email,
            Initialen = user.Initialen,
            Rolle = user.Rolle
        };

        public static User FromUserDto(UserDto dto) => new()
        {
            Id = dto.Id,
            UserName = dto.UserName,
            Vorname = dto.Vorname,
            Nachname = dto.Nachname,
            Email = dto.Email,
            Rolle = dto.Rolle,
            LetzterLogin = dto.LetzterLogin
        };
    }
}
