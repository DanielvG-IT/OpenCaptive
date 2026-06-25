namespace OpenCaptive.Infrastructure.Email;

public sealed class EmailOptions
{
  public string FromAddress { get; init; } = string.Empty;
  public string FromName { get; init; } = string.Empty;
  public SMTPEmailOptions SMTP { get; init; } = new();
  public TimeSpan Timeout { get; init; } = default;
}

public sealed class SMTPEmailOptions
{
  public string Host { get; init; } = string.Empty;
  public int Port { get; init; } = default;
  public string Username { get; init; } = string.Empty;
  public string Password { get; init; } = string.Empty;
  public bool EnableSsl { get; init; } = true;
}