using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Auth;

public static class DataProtectionServiceCollectionExtensions
{
  /// <summary>
  /// Persists the Data Protection key ring in Postgres so Identity-issued tokens survive
  /// restarts and are accepted by every replica.
  /// </summary>
  public static IServiceCollection AddOpenCaptiveDataProtection(this IServiceCollection services)
  {
    services
        .AddDataProtection()
        // The application name is baked into every purpose string. It must stay constant
        // across instances and deployments - changing it is equivalent to rotating the whole
        // key ring, instantly invalidating every outstanding verification and reset token.
        .SetApplicationName(ApplicationName)
        .PersistKeysToDbContext<OpenCaptiveDbContext>();

    // The ring is stored unencrypted, so ASP.NET logs "No XML encryptor configured" at
    // startup. That is accepted, not overlooked: the keys live in the same database as the
    // data they protect, so encrypting them with a certificate deployed alongside that
    // database would move the secret, not protect it. Add ProtectKeysWith* only once there
    // is a real key vault / HSM to hold the wrapping key.

    return services;
  }

  /// <summary>
  /// Data Protection purpose-string prefix. Never change this on a live deployment.
  /// </summary>
  private const string ApplicationName = "OpenCaptive";
}
