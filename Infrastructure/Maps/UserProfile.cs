using AutoMapper;
using Domain.Entities;
using Domain.Models;

namespace Infrastructure.Maps
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserInfoDto, UserInfo>();
            CreateMap<PlayerRankDto, PlayerRank>();
        }
    }
}
