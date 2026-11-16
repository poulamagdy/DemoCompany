using AutoMapper;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Identity;

namespace Demo.PL.Mapper
{
    public class RolesProfile : Profile
    {
        public RolesProfile()
        {
            CreateMap<ApplicationRoles,RolesViewModel>().ReverseMap();
        }
    }
}
