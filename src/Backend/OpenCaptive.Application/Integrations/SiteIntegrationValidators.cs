using FluentValidation;

namespace OpenCaptive.Application.Integrations;

public sealed class CreateSiteIntegrationInputValidator : AbstractValidator<CreateSiteIntegrationInput>
{
  public CreateSiteIntegrationInputValidator()
  {
    // Provider is a free string on the Domain entity (see SiteIntegration.cs) — decide whether
    // this should be constrained to a known-provider enum/registry once the Integrations
    // vendor projects (UniFi, Omada, ...) actually exist.
    RuleFor(x => x.Provider)
      .NotEmpty()
      .MaximumLength(50);

    RuleFor(x => x.DisplayName)
      .NotEmpty()
      .MaximumLength(200);
  }
}

public sealed class UpdateSiteIntegrationInputValidator : AbstractValidator<UpdateSiteIntegrationInput>
{
  public UpdateSiteIntegrationInputValidator()
  {
    RuleFor(x => x.DisplayName)
      .MaximumLength(200)
      .When(x => x.DisplayName is not null);
  }
}
