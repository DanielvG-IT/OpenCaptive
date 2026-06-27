namespace OpenCaptive.Infrastructure.Email.Components;

public enum ButtonVariant
{
  Primary,
  Secondary
}

/// <summary>
/// Model for the shared <c>_Button</c> component. Every button also renders the
/// raw URL beneath it, so no caller needs to repeat that markup.
/// </summary>
public sealed record ButtonModel(string Label, string Url, ButtonVariant Variant = ButtonVariant.Primary);
