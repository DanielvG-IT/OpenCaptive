using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Application.Auth.Models;

namespace OpenCaptive.Api.Endpoints;

public static class AuthEndpoints
{
  public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/auth").WithTags("Authentication");

    // Auth
    group.MapPost("/login", Login).WithName("AuthLogin");
    group.MapPost("/refresh", Refresh).WithName("AuthRefresh");
    // group.MapPost("/logout", Logout).RequireAuthorization().WithName("AuthLogout");

    // Registration
    group.MapPost("/register", Register).WithName("AuthRegister");

    // Email verification
    group.MapPost("/verify-email", VerifyEmail).WithName("AuthVerifyEmail");
    group.MapPost("/verify-email/resend", ResendVerificationEmail).WithName("AuthResendVerifyEmail");

    // Password reset
    group.MapPost("/forgot-password", ForgotPassword).WithName("AuthForgotPassword");
    group.MapPost("/reset-password", ResetPassword).WithName("AuthResetPassword");

    // MFA
    group.MapPost("/verify-mfa", VerifyTwoFactor).WithName("AuthVerifyMfa");
    group.MapPost("/verify-mfa/recovery-code", RedeemRecoveryCode).WithName("AuthRecoverVerifyMfa");

    return app;
  }

  private static async Task<Results<Ok<LoginResponse>, ValidationProblem, ProblemHttpResult>> Login(
    [FromBody] LoginInput input,
    [FromServices] IValidator<LoginInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.LoginAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<RegisterResponse>, ValidationProblem, ProblemHttpResult>> Register(
    [FromBody] RegisterInput input,
    [FromServices] IValidator<RegisterInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.RegisterAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<TokenResponse>, ValidationProblem, ProblemHttpResult>> Refresh(
    [FromBody] RefreshInput input,
    [FromServices] IValidator<RefreshInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.RefreshAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> VerifyEmail(
    [FromBody] VerifyEmailInput input,
    [FromServices] IValidator<VerifyEmailInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.VerifyEmailAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }

  private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> ResendVerificationEmail(
    [FromBody] ResendVerifyEmailInput input,
    [FromServices] IValidator<ResendVerifyEmailInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.ResendVerifyEmailAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }

  private static async Task<Results<Ok<TokenResponse>, ValidationProblem, ProblemHttpResult>> VerifyTwoFactor(
    [FromBody] VerifyMfaInput input,
    [FromServices] IValidator<VerifyMfaInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.VerifyTwoFactorAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<RedeemRecoveryCodeResponse>, ValidationProblem, ProblemHttpResult>> RedeemRecoveryCode(
    [FromBody] RedeemRecoveryCodeInput input,
    [FromServices] IValidator<RedeemRecoveryCodeInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.RedeemRecoveryCodeAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> ForgotPassword(
    [FromBody] ForgotPasswordInput input,
    [FromServices] IValidator<ForgotPasswordInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.ForgotPasswordAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }

  private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> ResetPassword(
    [FromBody] ResetPasswordInput input,
    [FromServices] IValidator<ResetPasswordInput> validator,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.ResetPasswordAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }
}
