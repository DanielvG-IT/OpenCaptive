namespace OpenCaptive.Application.Email;

public interface ITransactionalEmailProvider
{
  Task SendAsync(TransactionalEmail email, CancellationToken cancellationToken = default);
}

public sealed record TransactionalEmail(
  string To,
  string Subject,
  string HtmlBody,
  string? TextBody = null);
