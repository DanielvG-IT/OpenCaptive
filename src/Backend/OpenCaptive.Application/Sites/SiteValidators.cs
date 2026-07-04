using FluentValidation;
using OpenCaptive.Application.Common.Validation;

namespace OpenCaptive.Application.Sites;

public sealed class CreateSiteInputValidator : AbstractValidator<CreateSiteInput>
{
    public CreateSiteInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100)
            .MustBeValidSlug();

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .MaximumLength(64);
    }
}

public sealed class UpdateSiteInputValidator : AbstractValidator<UpdateSiteInput>
{
    public UpdateSiteInputValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name is not null);

        RuleFor(x => x.Slug)
            .MaximumLength(100)
            .MustBeValidSlug()
            .When(x => x.Slug is not null);

        RuleFor(x => x.TimeZone)
            .MaximumLength(64)
            .When(x => x.TimeZone is not null);
    }
}
