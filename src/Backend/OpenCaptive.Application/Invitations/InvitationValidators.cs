using FluentValidation;

namespace OpenCaptive.Application.Invitations;

public sealed class CreateInvitationInputValidator : AbstractValidator<CreateInvitationInput>
{
  public CreateInvitationInputValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty()
      .EmailAddress()
      .MaximumLength(320);

    RuleFor(x => x.Role).IsInEnum();
  }
}
