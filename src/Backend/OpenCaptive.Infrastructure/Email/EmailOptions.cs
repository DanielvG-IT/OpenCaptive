using OpenCaptive.Infrastructure.Common.Options;

namespace OpenCaptive.Infrastructure.Email;

public sealed class EmailOptions : IOptionsSection
{
  public static string SectionName { get; } = "Email";
  public required EmailFromOptions From { get; init; }
  public required SmtpOptions Smtp { get; init; }
  public EmailBrandingOptions Branding { get; init; } = new();
}

public sealed class EmailBrandingOptions
{
  public string BrandName { get; init; } = "OpenCaptive";
  public string WebsiteUrl { get; init; } = "https://opencaptive.com";
  public string SupportUrl { get; init; } = "https://opencaptive.com/support";
}

public sealed class EmailFromOptions
{
  public required string Name { get; init; }
  public required string Address { get; init; }
}

public sealed class SmtpOptions
{
  public required string Host { get; init; }
  public int Port { get; init; }
  public string? Username { get; init; }
  public string? Password { get; init; }
  public SmtpSecurityMode SecurityMode { get; init; } = SmtpSecurityMode.Auto;
  public TimeSpan Timeout { get; init; }
}

public enum SmtpSecurityMode
{
  /// <summary>Allows MailKit to decide based on server capabilities (often defaults to StartTls if available).</summary>
  Auto,
  /// <summary>No encryption (required for local dev tools like Papercut/MailHog).</summary>
  None,
  /// <summary>Elevates the connection to TLS via STARTTLS command.</summary>
  StartTls,
  /// <summary>Wraps the entire connection in SSL/TLS immediately upon connecting.</summary>
  SslOnConnect
}