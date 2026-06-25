namespace OpenCaptive.Application.Email.Models;

public sealed record VerifyEmailTemplateModel(
    string RecipientEmail,
    string VerificationUrl,
    TimeSpan TokenLifetime
);