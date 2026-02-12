using AutoMapper;
using Pavis.Application.DTOs.Auth;
using Pavis.Application.DTOs.Organizations;
using Pavis.Application.DTOs.Projects;
using Pavis.Domain.Entities;
using Pavis.Domain.ValueObjects;

namespace Pavis.Application.Mappings;

public class MappingProfile : Profile {
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Organization, OrganizationDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Project, ProjectDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ViabilityStatus, opt => opt.MapFrom(src => src.ViabilityStatus.ToString()))
            .ForMember(dest => dest.Organization, opt => opt.Ignore())
            .ForMember(dest => dest.Advisor, opt => opt.Ignore());

        CreateMap<ProjectProgress, ProjectProgressDTO>();
    }
}
