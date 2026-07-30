using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Organizations.Errors;

public static class OrganizationErrors
{
  public static Error NotFound(Guid id) =>
    Error.NotFound("Organization.NotFound", $"No organization was found with id '{id}'.");

  public static Error SlugAlreadyExists(string slug) =>
    Error.Conflict("Organization.SlugAlreadyExists", $"An organization with slug '{slug}' already exists.");
}
