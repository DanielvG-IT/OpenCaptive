using FluentValidation;
using OpenCaptive.Application.Organizations.Models;

namespace OpenCaptive.Application.Organizations.Validators;

public sealed class UpdateOrganizationInputValidator : AbstractValidator<UpdateOrganizationInput>
{
  public UpdateOrganizationInputValidator()
  {
    RuleFor(x => x.Name);

    RuleFor(x => x.Slug);
  }
}
