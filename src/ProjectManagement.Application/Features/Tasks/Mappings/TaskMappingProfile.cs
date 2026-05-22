using AutoMapper;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Features.Tasks.Mappings;

/// <summary>
/// AutoMapper profile for ProjectTask entity to TaskDto mapping.
/// </summary>
public sealed class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<ProjectTask, TaskDto>();
    }
}
