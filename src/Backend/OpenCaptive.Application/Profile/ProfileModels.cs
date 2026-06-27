namespace OpenCaptive.Application.Profile;

public sealed record ChangeTwoFactorStateInput(string Code);

public sealed record ProfileResponse(Guid Id, string Email, string FirstName, string LastName, bool EmailConfirmed, bool TwoFactorEnabled, DateTimeOffset? LastLoginAt);
public sealed record TwoFactorSetupResponse(string OtpAuthUri, string RawTotpKey);