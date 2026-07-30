using FluentValidation;

namespace OpenCaptive.Application.Auth.Validators;

public static class AuthValidatorExtensions
{
  public static IRuleBuilder<T, string> MustBeValidPassword<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
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