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
        .MustBeValidPassword();

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

public sealed class RedeemRecoveryCodeInputValidator : AbstractValidator<RedeemRecoveryCodeInput>
{
  public RedeemRecoveryCodeInputValidator()
  {
    RuleFor(x => x.ChallengeToken)
        .NotEmpty();

    RuleFor(x => x.RecoveryCode)
        .NotEmpty();
  }
}

public sealed class ResendVerifyEmailInputValidator : AbstractValidator<ResendVerifyEmailInput>
{
  public ResendVerifyEmailInputValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();
  }
}

public sealed class ForgotPasswordInputValidator : AbstractValidator<ForgotPasswordInput>
{
  public ForgotPasswordInputValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();
  }
}
public sealed class ResetPasswordInputValidator : AbstractValidator<ResetPasswordInput>
{
  public ResetPasswordInputValidator()
  {
    RuleFor(x => x.UserId)
        .NotEmpty();

    RuleFor(x => x.Token)
        .NotEmpty();

    RuleFor(x => x.NewPassword)
        .NotEmpty()
        .MustBeValidPassword();
  }
}