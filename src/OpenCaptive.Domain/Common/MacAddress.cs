using System.Text.RegularExpressions;

namespace OpenCaptive.Domain.Common;

public static partial class MacAddress
{
  [GeneratedRegex(@"^([0-9A-Fa-f]{2}[:.-]){5}([0-9A-Fa-f]{2})$")]
  private static partial Regex MacAddressRegEx();

  public static bool IsMacAddress(string? macAddress)
  {
    if (string.IsNullOrWhiteSpace(macAddress))
    {
      return false;
    }

    return MacAddressRegEx().IsMatch(macAddress);
  }
}