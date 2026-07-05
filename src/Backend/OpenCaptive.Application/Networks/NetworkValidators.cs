using FluentValidation;

namespace OpenCaptive.Application.Networks;

public sealed class CreateNetworkInputValidator : AbstractValidator<CreateNetworkInput>
{
  public CreateNetworkInputValidator()
  {
    RuleFor(x => x.SiteIntegrationId).NotEmpty();

    RuleFor(x => x.ProviderNetworkId)
      .NotEmpty()
      .MaximumLength(200);

    RuleFor(x => x.Name)
      .NotEmpty()
      .MaximumLength(200);
  }
}

public sealed class UpdateNetworkInputValidator : AbstractValidator<UpdateNetworkInput>
{
  public UpdateNetworkInputValidator()
  {
    RuleFor(x => x.Name)
      .MaximumLength(200)
      .When(x => x.Name is not null);
  }
}
