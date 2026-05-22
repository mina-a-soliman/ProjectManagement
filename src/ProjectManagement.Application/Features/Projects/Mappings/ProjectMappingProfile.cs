using AutoMapper;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Features.Projects.Mappings;

/// <summary>
/// AutoMapper profile for Project entity to ProjectDto mapping.
/// </summary>
public sealed class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ForCtorParam(nameof(ProjectDto.UserId),
                opt => opt.MapFrom(src => src.CreatedBy ?? Guid.Empty))
            .ForCtorParam(nameof(ProjectDto.TaskCount),
                opt => opt.MapFrom(src => src.Tasks.Count));
    }
}
