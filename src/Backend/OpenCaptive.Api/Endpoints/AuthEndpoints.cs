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
    group.MapPost("/login", Login);
    group.MapPost("/refresh", Refresh);
    // group.MapPost("/logout", Logout).RequireAuthorization();

    // Registration
    group.MapPost("/register", Register);

    // Email verification
    group.MapPost("/verify-email", VerifyEmail);
    // group.MapPost("/verify-email/resend", ResendVerificationEmail).RequireAuthorization();

    // Password reset
    // group.MapPost("/forgot-password", ForgotPassword);
    // group.MapPost("/reset-password", ResetPassword);

    // MFA
    // group.MapPost("/verify-mfa", VerifyMfa);

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

  private static async Task<Results<Ok<VerifyEmailReponse>, ValidationProblem, ProblemHttpResult>> VerifyEmail(
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

    return TypedResults.Ok(result.Value);
  }
}
