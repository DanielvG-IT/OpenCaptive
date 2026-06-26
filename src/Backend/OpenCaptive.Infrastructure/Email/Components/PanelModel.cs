namespace OpenCaptive.Infrastructure.Email.Components;

public sealed record PanelRow(string Label, string Value);

/// <summary>
/// Model for the shared <c>_Panel</c> component used to present structured
/// information such as invitation or login details as label/value rows.
/// </summary>
public sealed record PanelModel(IReadOnlyList<PanelRow> Rows, string? Title = null);
