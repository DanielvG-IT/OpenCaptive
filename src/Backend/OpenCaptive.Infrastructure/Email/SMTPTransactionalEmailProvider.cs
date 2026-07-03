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
    ArgumentNullException.ThrowIfNull(email.Bodies);
    ArgumentException.ThrowIfNullOrWhiteSpace(email.ToAddress);

    var message = new MimeMessage();

    message.From.Add(new MailboxAddress(_emailOptions.From.Name, _emailOptions.From.Address));
    message.To.Add(new MailboxAddress(email.ToName, email.ToAddress));
    message.Subject = email.Subject;
    message.Body = new BodyBuilder
    {
      HtmlBody = email.Bodies.HtmlBody,
      TextBody = email.Bodies.TextBody
    }.ToMessageBody();

    // Map domain enum to MailKit's SecureSocketOptions
    var sslMode = _emailOptions.Smtp.SecurityMode switch
    {
      SmtpSecurityMode.None => SecureSocketOptions.None,
      SmtpSecurityMode.StartTls => SecureSocketOptions.StartTls,
      SmtpSecurityMode.SslOnConnect => SecureSocketOptions.SslOnConnect,
      SmtpSecurityMode.Auto => SecureSocketOptions.Auto,
      _ => throw new InvalidOperationException($"An invalid or unsupported SMTP security mode '{_emailOptions.Smtp.SecurityMode}' was configured in EmailOptions.")
    };

    using var smtp = new SmtpClient();
    smtp.Timeout = (int)_emailOptions.Smtp.Timeout.TotalMilliseconds;

    await smtp.ConnectAsync(_emailOptions.Smtp.Host, _emailOptions.Smtp.Port, sslMode, cancellationToken);

    if (!string.IsNullOrWhiteSpace(_emailOptions.Smtp.Username) && !string.IsNullOrWhiteSpace(_emailOptions.Smtp.Password))
    {
      await smtp.AuthenticateAsync(_emailOptions.Smtp.Username, _emailOptions.Smtp.Password, cancellationToken);
    }

    await smtp.SendAsync(message, cancellationToken);
    await smtp.DisconnectAsync(quit: true, cancellationToken);

    _logger.LogInformation("Sent transactional email '{Subject}' to {Recipient} using security mode {Mode}", email.Subject, email.ToAddress, sslMode);
  }
}