using FluentValidation;
using OpenCaptive.Domain.Common;

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
      .Must(Slugs.CheckSlug)
      .WithMessage("Slug must be lowercase alphanumeric words separated by single hyphens.");
  }
}
