using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Invitations;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class InvitationEndpoints
{
  public static IEndpointRouteBuilder MapInvitationEndpoints(this IEndpointRouteBuilder app)
  {
    // Admin side — org-scoped management of pending invitations.
    var adminGroup = app.MapGroup("/organizations/invitations")
        .RequireAuthorization()
        .WithTags("Invitations");

    adminGroup.MapGet(string.Empty, GetAllInvitations).RequirePermission(Permissions.Invitations.Read);
    adminGroup.MapPost(string.Empty, CreateInvitation).RequirePermission(Permissions.Invitations.Create);
    adminGroup.MapGet("/{id:guid}", GetOneInvitation).RequirePermission(Permissions.Invitations.Read);
    adminGroup.MapDelete("/{id:guid}", RevokeInvitation).RequirePermission(Permissions.Invitations.Revoke);
    adminGroup.MapPost("/{id:guid}/resend", ResendInvitation).RequirePermission(Permissions.Invitations.Resend);

    // Invitee side — the caller isn't a member of the org yet, so this is identified by the
    // invitation id alone, not org-scoped permissions. Whether this requires the invitee to
    // already be authenticated (derive UserId from ICurrentUser) or to prove identity another
    // way (e.g. email match, a signup step first) is still an open design question — decide
    // before wiring RequireAuthorization() here.
    var inviteeGroup = app.MapGroup("/invitations")
        .WithTags("Invitations");

    inviteeGroup.MapGet("/{id:guid}", GetInvitation);
    inviteeGroup.MapPost("/{id:guid}/accept", AcceptInvitation);

    return app;
  }

  //* ===============
  //*  Admin side
  //* ===============
  private static async Task<Results<Ok<List<InvitationDto>>, ProblemHttpResult>> GetAllInvitations(
    [FromServices] IInvitationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetAllAsync(cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<InvitationDto>, ValidationProblem, ProblemHttpResult>> CreateInvitation(
    [FromBody] CreateInvitationInput input,
    [FromServices] IValidator<CreateInvitationInput> validator,
    [FromServices] IInvitationService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.CreateAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<InvitationDto>, ProblemHttpResult>> GetOneInvitation(
    [FromRoute] Guid id,
    [FromServices] IInvitationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetOneByIdAsync(id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> RevokeInvitation(
    [FromRoute] Guid id,
    [FromServices] IInvitationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.RevokeAsync(id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }

  private static async Task<Results<Ok<InvitationDto>, ProblemHttpResult>> ResendInvitation(
    [FromRoute] Guid id,
    [FromServices] IInvitationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.ResendAsync(id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  //* ===============
  //*  Invitee side
  //* ===============
  private static async Task<Results<Ok<InvitationDto>, ProblemHttpResult>> GetInvitation(
    [FromRoute] Guid id,
    [FromServices] IInvitationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetForInviteeAsync(id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> AcceptInvitation(
    [FromRoute] Guid id,
    [FromServices] IInvitationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.AcceptAsync(id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }
}
