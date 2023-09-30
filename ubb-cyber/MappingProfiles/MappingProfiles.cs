using AutoMapper;
using ubb_cyber.Models;
using ubb_cyber.ViewModels;

namespace ubb_cyber.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, PanelUserViewModel>();
            CreateMap<User, PanelEditUserViewModel>();
        }
    }
}
