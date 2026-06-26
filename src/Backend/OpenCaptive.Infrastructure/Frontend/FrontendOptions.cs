namespace OpenCaptive.Infrastructure.Frontend;

public sealed class FrontendOptions
{
  public const string SectionName = "Frontend";

  public required string ApplicationUrl { get; init; }
  public required string PortalUrl { get; init; }
}