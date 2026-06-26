namespace OpenCaptive.Infrastructure.Email.Components;

public enum AlertVariant
{
  Information,
  Success,
  Warning,
  Error
}

/// <summary>
/// Model for the shared <c>_Alert</c> component. The variant drives colour and
/// icon; meaning is never communicated by colour alone (an icon and title are
/// always rendered).
/// </summary>
public sealed record AlertModel(AlertVariant Variant, string Title, string? Description = null);
