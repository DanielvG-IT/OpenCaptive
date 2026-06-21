using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class AuthEndpoints
{
  public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/auth").WithTags("Authentication");

    group.MapPost("/login", Login);
    group.MapPost("/refresh", Refresh);
    // group.MapPost("/verify-mfa", VerifyMfa);
    // group.MapPost("/register", Register);

    group.MapGet("/me", Me).RequireAuthorization();

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

  private static async Task<Results<Ok<AuthResponse>, ValidationProblem, ProblemHttpResult>> Refresh(
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

  private static async Task<Results<Ok<MeResponse>, ValidationProblem, ProblemHttpResult>> Me(
    HttpContext context,
    [FromServices] IAuthService service,
    CancellationToken cancellationToken)
  {
    var userId = context.User.GetUserId();

    var result = await service.MeAsync(userId, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }
}
