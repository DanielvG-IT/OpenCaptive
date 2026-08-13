using Microsoft.Extensions.Logging;
using UniFi.Network.Client.Http;

namespace OpenCaptive.Integrations.UniFi.Internal;

internal sealed class UniFiApiExecutor(ILogger<UniFiApiExecutor> logger)
{
  public async Task<T?> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
  {
    try
    {
      cancellationToken.ThrowIfCancellationRequested();
      return await action();
    }
    catch (UniFiApiException ex)
    {
      logger.LogError(ex, "UniFi API request failed ({StatusCode})", ex.StatusCode);
      return default;
    }
  }
}