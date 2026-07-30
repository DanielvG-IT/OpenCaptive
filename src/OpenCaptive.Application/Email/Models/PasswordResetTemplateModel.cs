namespace OpenCaptive.Application.Email.Models;

public sealed record PasswordResetTemplateModel(
    string RecipientFirstName,
    string ResetUrl,
    TimeSpan TokenLifetime
);
