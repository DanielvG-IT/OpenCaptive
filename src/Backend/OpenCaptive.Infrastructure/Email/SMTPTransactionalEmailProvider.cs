using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OpenCaptive.Application.Email.Contracts;
using OpenCaptive.Application.Email.Models;

namespace OpenCaptive.Infrastructure.Email;

public sealed class SmtpTransactionalEmailProvider(IOptions<EmailOptions> emailOptions, ILogger<SmtpTransactionalEmailProvider> logger) : ITransactionalEmailProvider
{
  private readonly EmailOptions _emailOptions = emailOptions.Value;
  private readonly ILogger<SmtpTransactionalEmailProvider> _logger = logger;

  public async Task SendAsync(TransactionalEmail email, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(email);

    var message = new MimeMessage();

    message.From.Add(new MailboxAddress(_emailOptions.FromName, _emailOptions.FromAddress));
    message.To.Add(new MailboxAddress(email.ToName, email.ToAddress));
    message.Subject = email.Subject ?? throw new ArgumentException("Email subject cannot be null.", nameof(email));
    message.Body = new BodyBuilder
    {
      TextBody = email.TextBody,
      HtmlBody = email.HtmlBody
    }.ToMessageBody();

    // Use StartTls to prevent downgrade attacks
    var sslMode = _emailOptions.SMTP.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

    using var smtp = new SmtpClient();
    smtp.Timeout = (int)_emailOptions.Timeout.TotalMilliseconds;

    await smtp.ConnectAsync(_emailOptions.SMTP.Host, _emailOptions.SMTP.Port, sslMode, cancellationToken);

    if (!string.IsNullOrWhiteSpace(_emailOptions.SMTP.Username) && !string.IsNullOrWhiteSpace(_emailOptions.SMTP.Password))
    {
      await smtp.AuthenticateAsync(_emailOptions.SMTP.Username, _emailOptions.SMTP.Password, cancellationToken);
    }

    await smtp.SendAsync(message, cancellationToken);
    await smtp.DisconnectAsync(quit: true, cancellationToken);

    _logger.LogInformation("Sent transactional email '{Subject}' to {Recipient}", email.Subject, email.ToAddress);
  }
}