using Core.Models;
using FluentValidation;

namespace Core.Validators;

public sealed class IssueRequestValidator : AbstractValidator<IssueRequest>
{
    public IssueRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(65535);
    }
}