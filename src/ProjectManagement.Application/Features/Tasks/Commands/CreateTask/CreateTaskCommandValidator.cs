using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Validates the CreateTaskCommand.
/// </summary>
public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project Id is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid task status.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid task priority.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}
