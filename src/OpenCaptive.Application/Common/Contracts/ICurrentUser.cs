namespace OpenCaptive.Application.Common.Contracts;

public interface ICurrentUser
{
  Guid UserId { get; }

  Guid OrganizationId { get; }

  string Email { get; }
}