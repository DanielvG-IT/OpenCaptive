using OpenCaptive.Application.Common;
using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Application.Organizations;

public sealed class OrganizationService(IOrganizationRepository repository) : IOrganizationService
{
  private readonly IOrganizationRepository _repository = repository;

  public async Task<Result<OrganizationDto>> GetAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var organization = await _repository.GetByIdAsync(id, cancellationToken);
    if (organization is null)
    {
      return Result.Failure<OrganizationDto>(OrganizationErrors.NotFound(id));
    }

    return Result.Success(ToDto(organization));
  }

  public async Task<Result<OrganizationDto>> CreateAsync(CreateOrganizationInput input, CancellationToken cancellationToken = default)
  {
    if (await _repository.ExistsBySlugAsync(input.Slug, cancellationToken))
    {
      return Result.Failure<OrganizationDto>(OrganizationErrors.SlugAlreadyExists(input.Slug));
    }

    var organization = Organization.Create(input.Name, input.Slug);

    var result = await _repository.AddAsync(organization, cancellationToken);
    if (result.IsFailure)
    {
      return Result.Failure<OrganizationDto>(result.Error);
    }

    return Result.Success(ToDto(organization));
  }

  private static OrganizationDto ToDto(Organization organization) => new(organization.Id, organization.Name, organization.Slug);
}
