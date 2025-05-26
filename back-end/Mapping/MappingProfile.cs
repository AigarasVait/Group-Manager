using AutoMapper;
using back_end.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Group, GroupDto>();
        CreateMap<Group, GroupSimpleDto>()
            .ForMember(dest => dest.Balance, opt => opt.Ignore());
        CreateMap<User, UserDto>()
            .ForMember(user => user.Balance, opt => opt.Ignore());
        CreateMap<Transaction, TransactionDto>();
        CreateMap<Transaction, TransactionCreateDto>();
    }
}