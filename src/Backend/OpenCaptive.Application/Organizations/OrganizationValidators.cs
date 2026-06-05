using FluentValidation;

namespace OpenCaptive.Application.Organizations;

public sealed class CreateOrganizationInputValidator : AbstractValidator<CreateOrganizationInput>
{
  public CreateOrganizationInputValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .MaximumLength(200);

    RuleFor(x => x.Slug)
      .NotEmpty()
      .MaximumLength(100)
      .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
      .WithMessage("Slug must be lowercase alphanumeric words separated by single hyphens.");
  }
}