using OpenCaptive.Application.Common;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Application.Invitations;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Invitations;

public sealed class InvitationService(OpenCaptiveDbContext dbContext, ICurrentUser currentUser) : IInvitationService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly ICurrentUser _currentUser = currentUser;

  public Task<Result<List<InvitationDto>>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<InvitationDto>> GetOneByIdAsync(Guid invitationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<InvitationDto>> CreateAsync(CreateInvitationInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result> RevokeAsync(Guid invitationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<InvitationDto>> ResendAsync(Guid invitationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<InvitationDto>> GetForInviteeAsync(Guid invitationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result> AcceptAsync(Guid invitationId, AcceptInvitationInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
