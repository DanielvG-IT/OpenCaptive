using FluentValidation;

namespace OpenCaptive.Application.Portals;

public sealed class UpdatePortalInputValidator : AbstractValidator<UpdatePortalInput>
{
  public UpdatePortalInputValidator()
  {
    RuleFor(x => x.Name)
      .MaximumLength(200)
      .When(x => x.Name is not null);
  }
}

public sealed class CreatePortalVersionInputValidator : AbstractValidator<CreatePortalVersionInput>
{
  public CreatePortalVersionInputValidator()
  {
    RuleFor(x => x.Content).NotEmpty();
  }
}
