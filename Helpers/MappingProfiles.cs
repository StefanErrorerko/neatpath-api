using AutoMapper;
using NeatPath.Dto.Request;
using NeatPath.Dto.Response;
using NeatPath.Models;
using System.Drawing;

namespace NeatPath.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {

            CreateMap<User, UserResponseDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<UserChangePasswordDto, User>();
            
            CreateMap<Session, SessionResponseDto>();
            CreateMap<SessionCreateDto, Session>();
            CreateMap<SessionExpiringDateDto, Session>();

            CreateMap<Url, UrlResponseDto>();
            CreateMap<UrlCreateDto, Url>();
            CreateMap<UrlClicksDto, Url>();
        }
    }
}
