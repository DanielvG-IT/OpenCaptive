using FluentValidation;

namespace OpenCaptive.Application.Auth;

public sealed class RegisterInputValidator : AbstractValidator<RegisterInput>
{
  public RegisterInputValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();

    RuleFor(x => x.Password)
        .NotEmpty()
        .MinimumLength(12)
        .Matches("[A-Z]")
        .WithMessage("Password must contain an uppercase letter.")
        .Matches("[a-z]")
        .WithMessage("Password must contain a lowercase letter.")
        .Matches("[0-9]")
        .WithMessage("Password must contain a number.");
  }
}

public sealed class LoginInputValidator : AbstractValidator<LoginInput>
{
  public LoginInputValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();

    RuleFor(x => x.Password)
        .NotEmpty();
  }
}

public sealed class RefreshInputValidator : AbstractValidator<RefreshInput>
{
  public RefreshInputValidator()
  {
    RuleFor(x => x.RefreshToken)
        .NotEmpty();
  }
}

public sealed class VerifyMfaInputValidator : AbstractValidator<VerifyMfaInput>
{
  public VerifyMfaInputValidator()
  {
    RuleFor(x => x.ChallengeToken)
        .NotEmpty();

    RuleFor(x => x.Code)
        .NotEmpty()
        .Length(6)
        .Matches("^[0-9]{6}$");
  }
}