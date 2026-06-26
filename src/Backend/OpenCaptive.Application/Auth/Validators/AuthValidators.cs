using FluentValidation;
using OpenCaptive.Application.Auth.Models;

namespace OpenCaptive.Application.Auth.Validators;

public sealed class RegisterInputValidator : AbstractValidator<RegisterInput>
{
  public RegisterInputValidator()
  {
    RuleFor(x => x.OrganizationName)
        .NotEmpty()
        .MaximumLength(200);

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

public sealed class VerifyEmailInputValidator : AbstractValidator<VerifyEmailInput>
{
  public VerifyEmailInputValidator()
  {
    RuleFor(x => x.UserId)
        .NotEmpty();

    RuleFor(x => x.Token)
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
