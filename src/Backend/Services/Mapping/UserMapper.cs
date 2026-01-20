using AutoMapper;
using Data.Persistence.Documents;
using Shared.Domain.Models;
using Shared.Dtos;

namespace Services.Mapping
{
    public class UserMapper: Profile
    {
        public UserMapper() 
        {
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<UserDto, UserDocument>();
            CreateMap<UserDocument, UserDto>();

            CreateMap<UserDocument, User>();
            CreateMap<User, UserDocument>();
        }
    }
}
