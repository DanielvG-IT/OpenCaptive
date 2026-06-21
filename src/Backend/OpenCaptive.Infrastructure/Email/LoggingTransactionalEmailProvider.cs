using Microsoft.Extensions.Logging;
using OpenCaptive.Application.Email;

namespace OpenCaptive.Infrastructure.Email;

public sealed class LoggingTransactionalEmailProvider(ILogger<LoggingTransactionalEmailProvider> logger) : ITransactionalEmailProvider
{
  public Task SendAsync(TransactionalEmail email, CancellationToken cancellationToken = default)
  {
    logger.LogInformation(
      """
      =====================================
      TRANSACTIONAL EMAIL (development stub)
      To: {To}
      Subject: {Subject}

      {HtmlBody}
      =====================================
      """,
      email.To, email.Subject, email.HtmlBody);

    return Task.CompletedTask;
  }
}
