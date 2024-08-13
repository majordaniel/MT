
using AutoMapper;
using MTMiddleware.Data.Entities;
using MTMiddleware.Data.ViewModels;

namespace MTMiddleware.Identity.Auth;

public class AuthAutoMapperProfile : Profile
{
    public AuthAutoMapperProfile()
    {
        CreateMap<ApplicationUser, ApplicationUserViewModel>();
    }
}