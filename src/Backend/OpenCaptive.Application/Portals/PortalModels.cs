namespace OpenCaptive.Application.Portals;

public sealed record CreatePortalInput(string Name);
public sealed record UpdatePortalInput(string? Name);

public sealed record PortalDto(Guid Id, Guid NetworkId, string Name, bool IsPublished, Guid? PublishedVersionId);

// Content shape (drag & drop / theming per PRODUCT.md "Portal Builder") is undesigned —
// Domain.Portals.PortalVersion currently stores it as an opaque JSON string. Revisit this
// input/DTO once the builder's document schema exists.
public sealed record CreatePortalVersionInput(string Content);
public sealed record PortalVersionDto(Guid Id, Guid PortalId, int Version, bool IsPublished, string Content);
