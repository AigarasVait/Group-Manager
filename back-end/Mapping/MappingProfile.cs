using AutoMapper;
using back_end.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Group, GroupDto>();
        // Ignore Balance because it's calculated manually, not mapped directly
        CreateMap<Group, GroupSimpleDto>()
            .ForMember(dest => dest.Balance, opt => opt.Ignore());
        // Ignore Balance because it's calculated manually, not mapped directly
        CreateMap<User, UserDto>()
            .ForMember(user => user.Balance, opt => opt.Ignore());
        CreateMap<Transaction, TransactionDto>();
        CreateMap<Transaction, TransactionCreateDto>();
    }
}