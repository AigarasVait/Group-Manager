using AutoMapper;
using back_end.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Group, GroupDto>();
        CreateMap<User, UserDto>();
    }
}