namespace OpenCaptive.Infrastructure.Frontend;

public sealed class FrontendOptions
{
  public string BaseUrl { get; init; } = default!;
  public const string VerifyEmailPath = "/verify-email";
  public const string ResetPasswordPath = "/reset-password";
  public const string InvitePath = "/invite";
}