using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Profile;

namespace OpenCaptive.Api.Endpoints;

public static class ProfileEndpoints
{
  public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
  {
    // TODO Add Permissions to endpoints
    var group = app.MapGroup("/profile").WithTags("Profile").RequireAuthorization();

    group.MapGet(string.Empty, GetProfile).WithName("ProfileGet");
    // group.MapPatch(string.Empty, PatchProfile).WithName("ProfileUpdate");

    // group.MapPost("/avatar", UploadAvatar).WithName("ProfileUploadAvatar");
    // group.MapDelete("/avatar", DeleteAvatar).WithName("ProfileDeleteAvatar");

    group.MapPost("/mfa/setup", SetupTwoFactor).WithName("ProfileSetupMfa");
    group.MapPost("/mfa/enable", EnableTwoFactor).WithName("ProfileEnableMfa");
    group.MapPost("/mfa/disable", DisableTwoFactor).WithName("ProfileDisableMfa");
    // group.MapPost("/mfa/regenerate-recovery-codes", RegenerateRecoveryCodes).WithName("ProfileRegenerateRecoveryCodes");

    return app;
  }

  private static async Task<Results<Ok<ProfileResponse>, ValidationProblem, ProblemHttpResult>> GetProfile(
    HttpContext context,
    [FromServices] IProfileService service,
    CancellationToken cancellationToken)
  {
    var userId = context.User.GetUserId();

    var result = await service.GetProfileAsync(userId, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<TwoFactorSetupResponse>, ValidationProblem, ProblemHttpResult>> SetupTwoFactor(
    HttpContext context,
    [FromServices] IProfileService service,
    CancellationToken cancellationToken)
  {
    var userId = context.User.GetUserId();

    var result = await service.SetupTwoFactorAsync(userId, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Created<TwoFactorEnableResponse>, ValidationProblem, ProblemHttpResult>> EnableTwoFactor(
    HttpContext context,
    [FromBody] ChangeTwoFactorStateInput input,
    [FromServices] IValidator<ChangeTwoFactorStateInput> validator,
    [FromServices] LinkGenerator linkGenerator,
    [FromServices] IProfileService service,
    CancellationToken cancellationToken)
  {
    var userId = context.User.GetUserId();
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.EnableTwoFactorAsync(userId, input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Created(linkGenerator.GetPathByName("AuthVerifyMfa"), result.Value);
  }

  private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> DisableTwoFactor(
    HttpContext context,
    [FromBody] ChangeTwoFactorStateInput input,
    [FromServices] IValidator<ChangeTwoFactorStateInput> validator,
    [FromServices] IProfileService service,
    CancellationToken cancellationToken)
  {
    var userId = context.User.GetUserId();
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.DisableTwoFactorAsync(userId, input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }
}