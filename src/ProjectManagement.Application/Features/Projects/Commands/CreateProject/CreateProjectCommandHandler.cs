using AutoMapper;
using MediatR;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Handles creating a new project owned by the current authenticated user.
/// </summary>
public sealed class CreateProjectCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can create projects.");

        var project = Project.Create(request.Name, request.Description, userId);

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<ProjectDto>(project);

        return Result<ProjectDto>.SuccessResponse(dto, "Project created successfully.");
    }
}
