using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Application.Auth.Models;

namespace OpenCaptive.Api.Endpoints;

public static class ProfileEndpoints
{
  public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
  {
    // TODO Add Permissions to endpoints
    var group = app.MapGroup("/profile").WithTags("Profile").RequireAuthorization();

    group.MapGet(string.Empty, GetProfile);
    // group.MapPatch(string.Empty, PatchProfile);

    // group.MapPost("/avatar", UploadAvatar);
    // group.MapDelete("/avatar", DeleteAvatar);

    // group.MapPost("/change-password", ChangePassword);
    // group.MapPost("/change-email", ChangeEmail);

    // group.MapPost("/mfa/enable", EnableTwoFactor).RequireAuthorization();
    // group.MapPost("/mfa/disable", DisableTwoFactor).RequireAuthorization();
    // group.MapPost("/mfa/regenerate-recovery-codes", RegenerateRecoveryCodes).RequireAuthorization();

    return app;
  }

  // TODO Extract methode from AuthService into seperate service
  private static async Task<Results<Ok<MeResponse>, ValidationProblem, ProblemHttpResult>> GetProfile(
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