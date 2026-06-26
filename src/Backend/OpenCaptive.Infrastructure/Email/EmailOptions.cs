using OpenCaptive.Infrastructure.Common.Options;

namespace OpenCaptive.Infrastructure.Email;

public sealed class EmailOptions : IOptionsSection
{
  public static string SectionName { get; } = "Email";

  public required EmailFromOptions From { get; init; }

  public required SmtpOptions Smtp { get; init; }
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

  public bool EnableSsl { get; init; }

  public TimeSpan Timeout { get; init; }
}