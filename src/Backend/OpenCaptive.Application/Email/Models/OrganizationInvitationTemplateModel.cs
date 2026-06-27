namespace OpenCaptive.Application.Email.Models;

public sealed record OrganizationInvitationTemplateModel(
    string? RecipientFirstName,
    string OrganizationName,
    string InviterName,
    string RoleName,
    string InvitationUrl,
    TimeSpan TokenLifetime
);
