namespace OpenCaptive.Application.Email.Models;

public sealed record TransactionalEmail(
    string ToAddress,
    string? ToName,
    string Subject,
    TransactionalEmailBodies Bodies
);

public sealed record TransactionalEmailBodies(
    string HtmlBody,
    string? TextBody
);