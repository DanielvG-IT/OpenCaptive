namespace OpenCaptive.Domain.Integrations;

// Eventually this will live in Infrastructure
public sealed class IntegrationConfiguration
{
  public Guid IntegrationId { get; private set; }
  public string EncryptedConfiguration { get; private set; } = null!;
}