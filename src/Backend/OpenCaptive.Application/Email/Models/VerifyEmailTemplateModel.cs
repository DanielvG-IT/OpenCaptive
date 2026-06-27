namespace OpenCaptive.Application.Email.Models;

public sealed record VerifyEmailTemplateModel(
    string RecipientFirstName,
    string VerificationUrl,
    TimeSpan TokenLifetime
);