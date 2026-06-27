using FluentValidation;

namespace OpenCaptive.Application.Profile;

public sealed class ChangeTwoFactorStateInputValidator : AbstractValidator<ChangeTwoFactorStateInput>
{
  public ChangeTwoFactorStateInputValidator()
  {
    RuleFor(x => x.Code)
        .NotEmpty()
        .Length(6)
        .Must(c => c.All(char.IsDigit));
  }
}