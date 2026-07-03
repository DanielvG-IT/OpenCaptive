using FluentValidation;
using OpenCaptive.Application.Organizations.Models;
using OpenCaptive.Domain.Common;

namespace OpenCaptive.Application.Organizations.Validators;

public sealed class UpdateOrganizationInputValidator : AbstractValidator<UpdateOrganizationInput>
{
  public UpdateOrganizationInputValidator()
  {
    RuleFor(x => x.Name)
        .MaximumLength(200)
        .When(x => x.Name is not null);

    RuleFor(x => x.Slug)
        .MaximumLength(100)
        .Must(Slugs.CheckSlug)
        .When(x => x.Slug is not null)
        .WithMessage("Slug must be lowercase alphanumeric words separated by single hyphens.");
  }
}
