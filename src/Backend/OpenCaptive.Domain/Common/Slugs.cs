using System.Text.RegularExpressions;

namespace OpenCaptive.Domain.Common;

public static partial class Slugs
{
  [GeneratedRegex(@"^[a-z][a-z-]*[a-z]$")]
  private static partial Regex SlugRegex();

  public static bool CheckSlug(string input)
  {
    if (string.IsNullOrWhiteSpace(input))
    {
      return false;
    }

    return SlugRegex().IsMatch(input);
  }

  public static void ThrowIfInvalidSlug(string slug)
  {
    if (!CheckSlug(slug))
    {
      throw new ArgumentException("The provided slug is invalid. It must be lowercase, alphanumeric, and can only contain internal hyphens.", nameof(slug));
    }
  }
}