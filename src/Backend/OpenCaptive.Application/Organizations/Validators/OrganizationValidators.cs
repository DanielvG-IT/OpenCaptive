using FluentValidation;
using OpenCaptive.Application.Common.Validation;
using OpenCaptive.Application.Organizations.Models;

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
        .MustBeValidSlug()
        .When(x => x.Slug is not null);
  }
}
