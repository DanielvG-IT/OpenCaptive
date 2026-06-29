using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class EmailVerificationTokenProviderOptions : DataProtectionTokenProviderOptions
{
  public EmailVerificationTokenProviderOptions()
  {
    Name = TokenProviders.EmailVerification;
  }
}

public sealed class EmailVerificationTokenProvider<TUser>(
  IDataProtectionProvider dataProtectionProvider,
  IOptions<EmailVerificationTokenProviderOptions> options,
  ILogger<EmailVerificationTokenProvider<TUser>> logger)
  : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger)
  where TUser : class;

public sealed class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
{
  public PasswordResetTokenProviderOptions()
  {
    Name = TokenProviders.PasswordReset;
  }
}

public sealed class PasswordResetTokenProvider<TUser>(
  IDataProtectionProvider dataProtectionProvider,
  IOptions<PasswordResetTokenProviderOptions> options,
  ILogger<PasswordResetTokenProvider<TUser>> logger)
  : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger)
  where TUser : class;
