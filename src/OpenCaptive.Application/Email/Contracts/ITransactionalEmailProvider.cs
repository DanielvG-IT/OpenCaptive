using OpenCaptive.Application.Email.Models;

namespace OpenCaptive.Application.Email.Contracts;

public interface ITransactionalEmailProvider
{
  Task SendAsync(TransactionalEmail email, CancellationToken cancellationToken = default);
}