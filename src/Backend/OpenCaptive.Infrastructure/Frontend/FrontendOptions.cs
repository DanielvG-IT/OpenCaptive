using OpenCaptive.Infrastructure.Common.Options;

namespace OpenCaptive.Infrastructure.Frontend;

public sealed class FrontendOptions : IOptionsSection
{
  public static string SectionName { get; } = "Frontend";

  public required string ApplicationUrl { get; init; }
  public required string PortalUrl { get; init; }
}