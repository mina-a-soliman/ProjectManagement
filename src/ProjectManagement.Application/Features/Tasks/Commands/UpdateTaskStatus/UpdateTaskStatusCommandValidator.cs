using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

/// <summary>
/// Validates the UpdateTaskStatusCommand.
/// </summary>
public sealed class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task Id is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid task status.");
    }
}
