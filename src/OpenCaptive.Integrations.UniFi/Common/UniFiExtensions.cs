using UniFi.Network.Client.Http;

namespace OpenCaptive.Integrations.UniFi.Common;

internal static class UniFiExtensions
{
  public static bool IsNotFound(this UniFiApiException ex) => ex.StatusCode == System.Net.HttpStatusCode.NotFound;
}